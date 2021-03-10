import React, {createContext, ReactNode, useState} from "react";

export const LoginContext = createContext({
    isLoggedIn: false,
    isAdmin: false,
    // isUsernamePassword: "",
    logIn: () => {},
    logOut: () => {},
});

interface LoginManagerProps {
    children: ReactNode
}

export function LoginManager(props: LoginManagerProps): JSX.Element {
    const [loggedIn, setLoggedIn] = useState(false);
    // const [usernamePassword, setUsernamePassword] = useState("");
    // var usernamePassword = "";
    
    function logIn() {
        setLoggedIn(true);
    }
    // function logIn(encodedUsernamePassword: string) {
    //     setUsernamePassword(encodedUsernamePassword);
    //     usernamePassword = encodedUsernamePassword;
    //     setLoggedIn(true);
    // }
    
    function logOut() {
        setLoggedIn(false);
    }
    
    const context = {
        isLoggedIn: loggedIn,
        isAdmin: loggedIn,
        // isUsernamePassword: usernamePassword,
        logIn: logIn,
        logOut: logOut,
    };
    
    return (
        <LoginContext.Provider value={context}>
            {props.children}
        </LoginContext.Provider>
    );
}