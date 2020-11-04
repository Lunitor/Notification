import * as React from 'react';
import { connect } from 'react-redux';
import { RouteComponentProps } from 'react-router';
import { ApplicationState } from '../../store';
import * as CreateEmailStore from '../../store/CreateEmail';
import { Tab, Tabs, TabList, TabPanel } from 'react-tabs';
import EmailBody from './EmailBody';
import TagButton from './TagButton';
import SelectEmailType from './SelectEmailType';
import EmailBodyPreview from './EmailBodyPreview';
import SubjectInput from './SubjectInput';
import { CreateEmailRequest, BadCreateEmailResponse, OkCreateEmailResponse } from '../../models/CreateEmailRequest';
import { StringHtml } from '../../utility/StringHtml';
import { AlertBox } from '../AlertBox';

// At runtime, Redux will merge together...
type CreateEmailProps =
    CreateEmailStore.CreateEmailState // ... state we've requested from the Redux store
    & typeof CreateEmailStore.actionCreators // ... plus action creators we've requested
    & RouteComponentProps; // ... plus incoming routing parameters

type CreateEmailState = {
    alerts: Array<[string, string]>
};

class CreateEmail extends React.PureComponent<CreateEmailProps, CreateEmailState> {
    constructor(props: CreateEmailProps) {
        super(props);

        this.state = {
            alerts: new Array<[string, string]>()
        };
    }

    // This method is called when the component is first added to the document
    public componentDidMount() {
        this.ensureDataFetched();
    }

    public render() {
        var alerts: any[] = [];

        if (this.state.alerts.length > 0) {
            this.state.alerts.forEach(alert => {
                alerts.push(<AlertBox variant={alert[0]} message={alert[1]} />);
            })
        }

        return (
            <React.Fragment>
                <form onSubmit={async (e) => { await this.handleSubmit(e); }}>
                    <div className="form-group row">
                        <label htmlFor="emailTypeSelector" className="col-sm-1 col-form-label">Type</label>
                        <div className="col-sm-11">
                            <SelectEmailType id="emailTypeSelector" />
                        </div>
                    </div>
                    <div className="form-group row">
                        <label htmlFor="subjectInput" className="col-sm-1 col-form-label">Subject</label>
                        <div className="col-sm-11">
                            <SubjectInput/>
                        </div>
                    </div>
                    <div className="form-group row">
                        <div className="col-9">
                            <Tabs>
                                <TabList>
                                    <Tab>Edit</Tab>
                                    <Tab>Preview</Tab>
                                </TabList>

                                <TabPanel>
                                    <EmailBody />
                                </TabPanel>
                                <TabPanel>
                                    <EmailBodyPreview />
                                </TabPanel>
                            </Tabs>
                        </div>
                        <div className="col-3">
                            {this.props.selectedEmailType.tags.map((tag: CreateEmailStore.Tag) =>
                                <div className="row">
                                    <div className="col-12">
                                        <TagButton tagName={tag.name} />
                                    </div>
                                </div>
                            )}
                        </div>
                    </div>
                    <div className="form-group row">
                        <div className="col-12">
                            <button type="submit" className="btn btn-primary btn-lg btn-block">Send</button>
                        </div>
                    </div>
                </form>
                { alerts }
                </React.Fragment>
            );
    }

    private async handleSubmit(event: React.FormEvent<HTMLFormElement>) {
        event.persist();
        event.preventDefault();

        const requestPayload: CreateEmailRequest = {
            type: this.props.selectedEmailType.name,
            subject: this.props.emailSubject,
            body: StringHtml.replaceNewLineWithBrTag(this.props.emailBody)
        };

        const response = await fetch('/api/sendemail', {
            method: 'POST',
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(requestPayload)
        })

        var resultMessages = new Array<[string, any]>();
        if (response.ok) {
            var okResponse = await (response.json() as Promise<OkCreateEmailResponse>);

            this.AddOkResponseMessages(okResponse, resultMessages);

            console.log("POST to /api/sendemail -> ok(200)");
        }
        else {
            var badResponse = await (response.json() as Promise<BadCreateEmailResponse>);

            this.AddBadResponseMessages(resultMessages, badResponse);

            var errorsString = this.ExctractErrorMessagesAsString(badResponse);
            console.error("POST to /api/sendemail -> " + response.status + "(" + response.statusText + ")\n" + errorsString);
        }

        this.setState({
            alerts: resultMessages
        });
    }


    private ensureDataFetched() {
        this.props.requestEmailTypes();
    }

    private AddOkResponseMessages(okResponse: OkCreateEmailResponse, resultMessages: [string, any][]) {
        const sentEmailCount = okResponse.results.filter(result => result.isSuccess).length;
        resultMessages.push(["success", "Email created - " + sentEmailCount + "/" + okResponse.results.length + " of emails sent"]);

        var failedEmailsMessage: any[] = [];
        okResponse.results.forEach(sendingEmailResult => {
            if (!sendingEmailResult.isSuccess) {
                failedEmailsMessage.push(
                    <span style={{ fontSize: "small" }}>
                        &nbsp; &nbsp; &nbsp; &nbsp;
                        {sendingEmailResult.emailAddress}
                    </span>,
                    <br />);
            }
        });
        if (failedEmailsMessage.length > 0) {
            failedEmailsMessage.unshift("Failed to send emails to:", <br />);
            resultMessages.push(["warning", failedEmailsMessage]);
        }
    }

    private AddBadResponseMessages(resultMessages: [string, any][], badResponse: BadCreateEmailResponse) {
        resultMessages.push(["danger", "Failed to create email!"]);

        const errorKeys: string[] = Object.keys(badResponse.errors);
        const errors: string[][] = Object.values(badResponse.errors);

        var errorMessages: any[] = [];
        errorKeys.forEach((errorKey: string, index: number) => {
            errorMessages.push(errorKey + ":", <br />);
            errors[index].forEach((error: string) => {
                errorMessages.push(
                    <span style={{ fontSize: "small" }}>
                        &nbsp; &nbsp; &nbsp; &nbsp;
                        {error}
                    </span>,
                    <br />);
            });
        });

        resultMessages.push(["danger", errorMessages]);
    }

    private ExctractErrorMessagesAsString(responseBody: BadCreateEmailResponse): string {
        var errorsString = "";

        const errorKeys: string[] = Object.keys(responseBody.errors);
        const errors: string[][] = Object.values(responseBody.errors);

        errorKeys.forEach((errorKey: string, index: number) => {
            errorsString += errorKey + ":\n";
            errors[index].forEach((error: string) => {
                errorsString += error + "\n";
            });
        });
        return errorsString;
    }
}

export default connect(
    (state: ApplicationState) => state.createEmail, // Selects which state properties are merged into the component's props
    CreateEmailStore.actionCreators // Selects which action creators are merged into the component's props
)(CreateEmail as any);
