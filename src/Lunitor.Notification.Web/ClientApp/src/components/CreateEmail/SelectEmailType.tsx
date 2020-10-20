import * as React from 'react';
import { connect } from 'react-redux';
import { RouteComponentProps } from 'react-router';
import { ApplicationState } from '../../store';
import * as CreateEmailStore from '../../store/CreateEmail';

interface SelectEmailTypeOwnProps {
    id: string
};

// At runtime, Redux will merge together...
type SelectEmailTypeProps =
    SelectEmailTypeOwnProps
    & CreateEmailStore.CreateEmailState // ... state we've requested from the Redux store
    & typeof CreateEmailStore.actionCreators // ... plus action creators we've requested
    & RouteComponentProps; // ... plus incoming routing parameters

class SelectEmailType extends React.PureComponent<SelectEmailTypeProps> {

    constructor(props: Readonly<SelectEmailTypeProps>) {
        super(props);

        this.handleSelectEmailType = this.handleSelectEmailType.bind(this);
    }

    public render() {
        return (
            <select id={this.props.id} onChange={this.handleSelectEmailType} className="form-control">
                {this.props.emailTypes.map((emailType: CreateEmailStore.EmailType) =>
                    <option value={emailType.name}>{emailType.name}</option>
                )}
            </select>
        );
    }

    private handleSelectEmailType(event: React.ChangeEvent<HTMLSelectElement>) {
        event.preventDefault();

        const selectedEmailTypeName = event.currentTarget.value;
        const selectedEmailType = this.props.emailTypes.find(emailType => emailType.name === selectedEmailTypeName);

        if (selectedEmailType !== undefined)
            this.props.selectEmailType(selectedEmailType);
    }
}

export default connect(
    (state: ApplicationState, ownProps: SelectEmailTypeOwnProps) => state.createEmail, // Selects which state properties are merged into the component's props
    CreateEmailStore.actionCreators // Selects which action creators are merged into the component's props
)(SelectEmailType as any);
