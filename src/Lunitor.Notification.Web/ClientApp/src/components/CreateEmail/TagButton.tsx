import * as React from 'react';
import { connect } from 'react-redux';
import { RouteComponentProps } from 'react-router';
import { ApplicationState } from '../../store';
import * as CreateEmailStore from '../../store/CreateEmail';

interface TagButtonOwnProps {
    tagName: string
}

// At runtime, Redux will merge together...
export type TagButtonProps =
    TagButtonOwnProps
    & CreateEmailStore.CreateEmailState // ... state we've requested from the Redux store
    & typeof CreateEmailStore.actionCreators // ... plus action creators we've requested
    & RouteComponentProps<{}>; // ... plus incoming routing parameters

class TagButton extends React.PureComponent<TagButtonProps> {

    constructor(props: TagButtonProps) {
        super(props);

        this.handleTagClick = this.handleTagClick.bind(this);
    }

    public render() {
        return (
            <button className="btn btn-secondary btn-sm btn-block"
                onClick={this.handleTagClick}>
                {this.props.tagName}
            </button>
        );
    }

    private handleTagClick(event: React.MouseEvent<HTMLButtonElement>) {
        event.preventDefault();

        const tag = this.props.selectedEmailType.tags.find(tag => tag.name === this.props.tagName);
        const tagPlaceholder = tag === undefined ? "" : tag.placeholder;
        const begining = this.props.emailBody.slice(0, this.props.cursorPositionInEmailBody);
        const end = this.props.emailBody.slice(this.props.cursorPositionInEmailBody);

        this.props.modifyEmailBody(begining + tagPlaceholder + end)
    }
}

export default connect(
    (state: ApplicationState, ownProps: TagButtonOwnProps) => state.createEmail, // Selects which state properties are merged into the component's props
    CreateEmailStore.actionCreators // Selects which action creators are merged into the component's props
)(TagButton as any);



