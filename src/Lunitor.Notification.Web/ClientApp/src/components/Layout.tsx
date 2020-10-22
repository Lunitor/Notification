import * as React from 'react';
import { Container} from 'reactstrap';
import NavMenu from './NavMenu';

export default (props: { children?: React.ReactNode }) => (
    <React.Fragment>
        <div className="d-flex">
            <NavMenu />
            <div className="main">
                <div className="top-row px-4"></div>
                <Container className="content">
                    {props.children}
                </Container>
            </div>
        </div>
    </React.Fragment>
);
