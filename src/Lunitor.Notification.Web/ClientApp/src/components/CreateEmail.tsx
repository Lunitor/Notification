import * as React from 'react';
import { connect } from 'react-redux';
import { RouteComponentProps } from 'react-router';
import { ApplicationState } from '../store';
import * as CreateEmailStore from '../store/CreateEmail';
import { FormEvent } from 'react';


// At runtime, Redux will merge together...
type CreateEmailProps =
    CreateEmailStore.CreateEmailState // ... state we've requested from the Redux store
    & typeof CreateEmailStore.actionCreators // ... plus action creators we've requested
    & RouteComponentProps; // ... plus incoming routing parameters

type CreateEmailState = {
    emailBody: string,
    cursorPositionInEmailBody: number
}

class CreateEmail extends React.PureComponent<CreateEmailProps, CreateEmailState> {
    input!: HTMLTextAreaElement | null;

    constructor(props: Readonly<CreateEmailProps>) {
        super(props);

        this.handleSelectEmailType = this.handleSelectEmailType.bind(this);
        this.handleTagClick = this.handleTagClick.bind(this);
        this.handleEmailBodyChange = this.handleEmailBodyChange.bind(this);
        this.handleEmailBodyClick = this.handleEmailBodyClick.bind(this);
        this.handleEmailBodyKeyUp = this.handleEmailBodyKeyUp.bind(this);

        this.state = {
            emailBody: "",
            cursorPositionInEmailBody: 0
        };
    }
    // This method is called when the component is first added to the document
    public componentDidMount() {
    this.ensureDataFetched();
    }

    // This method is called when the route parameters change
    public componentDidUpdate() {
    //this.ensureDataFetched();
    }

    public render() {
        return (
            <React.Fragment>
                <form onSubmit={this.handleSubmit}>
                    <div className="form-group row">
                        <label htmlFor="emailTypeSelector" className="col-sm-1 col-form-label">Type</label>
                        <div className="col-sm-11">
                            <select id="emailTypeSelector" onChange={this.handleSelectEmailType} className="form-control">
                                {this.props.emailTypes.map((emailType: CreateEmailStore.EmailType) =>
                                    <option value={emailType.name}>{emailType.name}</option>
                                )}
                            </select>
                        </div>
                    </div>
                    <div className="form-group row">
                        <label htmlFor="subjectInput" className="col-sm-1 col-form-label">Subject</label>
                        <div className="col-sm-11">
                            <input id="subjectInput" type="text" className="form-control"/>
                        </div>
                    </div>
                    <div className="form-group row">
                        <div className="col-9">
                            <textarea rows={10}
                                cols={90}
                                wrap="soft"
                                value={this.state.emailBody}
                                ref={el => this.input = el}
                                onChange={this.handleEmailBodyChange}
                                onClick={this.handleEmailBodyClick}
                                onKeyUp={this.handleEmailBodyKeyUp}
                                className="form-control"></textarea>
                        </div>
                        <div className="col-3">
                            {this.props.selectedEmailType.tags.map((tag: CreateEmailStore.Tag) =>
                                <div className="row">
                                    <div className="col-12">
                                        <button className="btn btn-secondary btn-sm btn-block"
                                                onClick={(e) => this.handleTagClick(tag.name, e)}>
                                            {tag.name}
                                        </button>
                                    </div>
                                </div>
                            )}
                        </div>
                    </div>
                    <div className="form-group row">
                        <div className="col-12">
                            <button type="button" className="btn btn-primary btn-lg btn-block">Send</button>
                        </div>
                    </div>
                </form>
            </React.Fragment>
            );
    }

    private handleSelectEmailType(event: React.ChangeEvent<HTMLSelectElement>) {
        event.preventDefault();

        const selectedEmailTypeName = event.currentTarget.value;
        const selectedEmailType = this.props.emailTypes.find(emailType => emailType.name === selectedEmailTypeName);

        if (selectedEmailType != undefined)
            this.props.selectEmailType(selectedEmailType);
    }

    private handleTagClick(tagName: string, event: React.MouseEvent<HTMLButtonElement>) {
        event.preventDefault();
        console.log(tagName + " clicked");

        const tag = this.props.selectedEmailType.tags.find(tag => tag.name == tagName);
        const tagPlaceholder = tag === undefined ? "" : tag.placeholder;
        const begining = this.state.emailBody.slice(0, this.state.cursorPositionInEmailBody);
        const end = this.state.emailBody.slice(this.state.cursorPositionInEmailBody);

        this.setState({
            emailBody: begining + tagPlaceholder + end,
        });
    }

    private handleEmailBodyChange(event: React.ChangeEvent<HTMLTextAreaElement>) {
        event.preventDefault();

        var cursorPosition = this.state.cursorPositionInEmailBody;
        if (this.input !== null) {
            cursorPosition = this.input.selectionStart;
            console.log("handle change - cursor at: " + cursorPosition);
        }

        this.setState({
            emailBody: event.currentTarget.value,
            cursorPositionInEmailBody: cursorPosition
        });
    }

    private handleEmailBodyClick(event: React.MouseEvent<HTMLTextAreaElement, MouseEvent>) {
        if (this.input !== null) {
            const cursorPosition = this.input.selectionStart;
            console.log("handle click - cursor at: " + cursorPosition);
            this.setState({
                cursorPositionInEmailBody: cursorPosition
            });
        }
    }

    private handleEmailBodyKeyUp(event: React.KeyboardEvent<HTMLTextAreaElement>) {
        if (this.input !== null && ["ArrowRight", "ArrowLeft", "ArrowUp", "ArrowDown"].includes(event.key)) {
            const cursorPosition = this.input.selectionStart;
            console.log("handle arrow keys - cursor at: " + cursorPosition);
            this.setState({
                cursorPositionInEmailBody: cursorPosition
            });
        }
    }

    private handleSubmit: ((event: FormEvent<HTMLFormElement>) => void) | undefined;

    private ensureDataFetched() {
        this.props.requestEmailTypes();
    }
}

export default connect(
    (state: ApplicationState) => state.createEmail, // Selects which state properties are merged into the component's props
    CreateEmailStore.actionCreators // Selects which action creators are merged into the component's props
)(CreateEmail as any);
