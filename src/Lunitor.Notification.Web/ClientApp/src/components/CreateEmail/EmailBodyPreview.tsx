import * as React from 'react';
import { connect } from 'react-redux';
import { RouteComponentProps } from 'react-router';
import { ApplicationState } from '../../store';
import * as CreateEmailStore from '../../store/CreateEmail';
import ReactHtmlParser from 'react-html-parser';


// At runtime, Redux will merge together...
type EmailBodyPreviewProps =
    CreateEmailStore.CreateEmailState // ... state we've requested from the Redux store
    & RouteComponentProps<{}>; // ... plus incoming routing parameters

class EmailBodyPreview extends React.PureComponent<EmailBodyPreviewProps> {
    public render() {
        return (
            ReactHtmlParser(this.props.emailBody)
        );
    }
}

export default connect(
    (state: ApplicationState) => state.createEmail, // Selects which state properties are merged into the component's props
    null
)(EmailBodyPreview as any);

