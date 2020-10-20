import * as React from 'react';
import { connect } from 'react-redux';
import { RouteComponentProps } from 'react-router';
import { ApplicationState } from '../../store';
import * as CreateEmailStore from '../../store/CreateEmail';


// At runtime, Redux will merge together...
type EmailBodyProps =
    CreateEmailStore.CreateEmailState // ... state we've requested from the Redux store
    & typeof CreateEmailStore.actionCreators // ... plus action creators we've requested
    & RouteComponentProps<{}>; // ... plus incoming routing parameters

class EmailBody extends React.PureComponent<EmailBodyProps> {
    input!: HTMLTextAreaElement | null;

    constructor(props: Readonly<EmailBodyProps>) {
        super(props);

        this.handleEmailBodyChange = this.handleEmailBodyChange.bind(this);
        this.handleEmailBodyClick = this.handleEmailBodyClick.bind(this);
        this.handleEmailBodyKeyUp = this.handleEmailBodyKeyUp.bind(this);
    }

    public render() {
        return (
            <textarea rows={10}
                cols={90}
                wrap="soft"
                value={this.props.emailBody}
                ref={el => this.input = el}
                onChange={this.handleEmailBodyChange}
                onClick={this.handleEmailBodyClick}
                onKeyUp={this.handleEmailBodyKeyUp}
                className="form-control"></textarea>
        );
    }

    private handleEmailBodyChange(event: React.ChangeEvent<HTMLTextAreaElement>) {
        event.preventDefault();

        var cursorPosition = this.props.cursorPositionInEmailBody;
        if (this.input !== null) {
            cursorPosition = this.input.selectionStart;
        }

        this.props.modifyEmailBody(event.currentTarget.value)
        this.props.updateCursorPositionInEmailBody(cursorPosition);
    }

    private handleEmailBodyClick(event: React.MouseEvent<HTMLTextAreaElement, MouseEvent>) {
        if (this.input !== null) {
            const cursorPosition = this.input.selectionStart;
            this.props.updateCursorPositionInEmailBody(cursorPosition);
        }
    }

    private handleEmailBodyKeyUp(event: React.KeyboardEvent<HTMLTextAreaElement>) {
        if (this.input !== null && ["ArrowRight", "ArrowLeft", "ArrowUp", "ArrowDown"].includes(event.key)) {
            const cursorPosition = this.input.selectionStart;
            this.props.updateCursorPositionInEmailBody(cursorPosition);
        }
    }
}

export default connect(
    (state: ApplicationState) => state.createEmail, // Selects which state properties are merged into the component's props
    CreateEmailStore.actionCreators // Selects which action creators are merged into the component's props
)(EmailBody as any);

