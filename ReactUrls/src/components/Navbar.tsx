import { Link } from 'react-router-dom';
import { useAppDispatch, useAppSelector } from '../hooks/reduxHooks';
import { loginUser, logout } from '../features/user/UserSlice';
import type { LoginRequest } from '../types/LoginRequest';
import '../styles/navbar.css'

const adminLogin = import.meta.env.VITE_APP_ADMIN_LOGIN;
const adminPassword = import.meta.env.VITE_APP_ADMIN_PASSWORD;

function Navbar() {
    const dispatch = useAppDispatch();
    const { isLoggedIn, userData } = useAppSelector((state) => state.user);

    const loginAction = () => {
        const loginRequest: LoginRequest = {
            username: adminLogin,
            password: adminPassword,
        };

        dispatch(loginUser(loginRequest));
    };

    const logoutAction = () => {
        dispatch(logout());
    };
    
    return (
        <nav className="navbar navbar-expand-lg navbar-light bg-light">
            <div className="container-fluid px-3">
                <Link className="navbar-brand" to="/">UrlShortener</Link>

                <div className="navbar-collapse">
                    <ul className="navbar-nav me-auto">
                        <li className="nav-item">
                            <Link className="nav-link" to="/urls">Urls</Link>
                        </li>
                    </ul>

                    <ul className="navbar-nav">
                        {isLoggedIn ? (
                        <>
                            <li className="nav-item">
                                <span className="nav-link">
                                    Hello, {userData?.nickname}
                                </span>
                            </li>
                            <li className="nav-item">
                                <button
                                    className="btn btn-link nav-link"
                                    onClick={logoutAction}>
                                    Logout
                                </button>
                            </li>
                        </>
                        ) : (
                            <li className="nav-item">
                                <button
                                    className="btn btn-link nav-link"
                                    onClick={loginAction}>
                                        Login
                                </button>
                            </li>
                        )}
                    </ul>
                </div>
            </div>
        </nav>
    );
};

export default Navbar;
