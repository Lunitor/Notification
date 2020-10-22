import * as React from 'react';
import { Navbar, NavItem, NavLink, Nav, NavbarBrand } from 'reactstrap';
import { Link } from 'react-router-dom';
import './NavMenu.css';

export default class NavMenu extends React.PureComponent<{}> {
    public render() {
        return (
            <div className="sidebar box-shadow">
                <Navbar className="top-row pl-4" dark>
                    <NavbarBrand href="/">Lunitor.Notification</NavbarBrand>
                </Navbar>
                <Nav vertical>
                    <NavItem>
                        <NavLink tag={Link} to="/counter">
                            <span className="oi oi-timer" aria-hidden="true"></span>
                            Counter
                        </NavLink>
                    </NavItem>
                    <NavItem>
                        <NavLink tag={Link} to="/email/create">
                            <span className="oi oi-envelope-closed" aria-hidden="true"></span>
                            Create email
                        </NavLink>
                    </NavItem>
                </Nav>
            </div>
        );
    }
}
