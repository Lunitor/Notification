import * as React from 'react';
import { Alert } from 'reactstrap';

interface AlertBoxProps {
    variant: string;
    message: any;
}

export class AlertBox extends React.PureComponent<AlertBoxProps> {
    public render() {
        return (
            <Alert color={this.props.variant}>
                {this.props.message}
            </Alert>
            );
    }
}