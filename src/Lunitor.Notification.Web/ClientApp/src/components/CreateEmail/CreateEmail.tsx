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
import { CreateEmailRequest } from '../../models/CreateEmailRequest';
import { StringHtml } from '../../utility/StringHtml';

// At runtime, Redux will merge together...
type CreateEmailProps =
    CreateEmailStore.CreateEmailState // ... state we've requested from the Redux store
    & typeof CreateEmailStore.actionCreators // ... plus action creators we've requested
    & RouteComponentProps; // ... plus incoming routing parameters

class CreateEmail extends React.PureComponent<CreateEmailProps> {
    constructor(props: CreateEmailProps) {
        super(props);
    }

    // This method is called when the component is first added to the document
    public componentDidMount() {
        this.ensureDataFetched();
    }

    public render() {
        return (
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

        const result = await fetch('/api/sendemail', {
            method: 'POST',
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(requestPayload)
        })

        const response = await result.json();

        console.log(response);
    }

    private ensureDataFetched() {
        this.props.requestEmailTypes();
    }
}

export default connect(
    (state: ApplicationState) => state.createEmail, // Selects which state properties are merged into the component's props
    CreateEmailStore.actionCreators // Selects which action creators are merged into the component's props
)(CreateEmail as any);
