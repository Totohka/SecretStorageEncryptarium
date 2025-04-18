import { useState } from "react";
import Input from "../../components/Input/Input";
import s from "./style.module.css";
import Button from "../../components/Button/Button";
import icon_app from "../../assets/icons/IconApp.svg";
import Link from "../../components/Link/Link";
import axios from "axios";
import { useNavigate } from "react-router";
import AuthCode from "../AuthCode/AuthCode";
import { toAuthorize } from "../../utils/authClient";

const Login = () => {
    axios.defaults.withCredentials = true;
    const [login, setLogin] = useState('');
    const [pass, setPass] = useState('');
    const [show, setShow] = useState(false);
    // const navigate = useNavigate();
    const toAuth = async() =>{
        console.log(login,' ', pass);
        const result = await toAuthorize({login:login, pass:pass});
        console.log(result);
        if (result.isSuccess) setShow(true);
        else console.log(result.error);
    }
    return (<>{!show?
        <div className={s.container}>
            <div className={s.form}>
                <img className={s.logo} src={icon_app} />
                <p className={s.title}>Авторизация</p>
                <Input type="text" alt="Логин" onChange={(e)=>setLogin(e.target.value)}/>
                <Input type="password" alt="Пароль" onChange={(e)=>setPass(e.target.value)}/>
                <div className={s.block}>
                    <Button text="Войти" onClick={toAuth}/>
                    <Link title="Зарегистрироваться" path="/registration"/>
                </div>
            </div>
        </div>
        :<AuthCode login={login} pass={pass}/>
        }
    </>);
}

export default Login;