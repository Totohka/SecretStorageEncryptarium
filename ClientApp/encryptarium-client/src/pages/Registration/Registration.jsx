import { useState } from "react";
import Input from "../../components/Input/Input";
import s from "./style.module.css";
import Button from "../../components/Button/Button";
import icon_app from "../../assets/icons/IconApp.svg";
import Link from "../../components/Link/Link";
import axios from "axios";
import { useNavigate } from "react-router";

const Registration = () => {
    const [email, setEmail] = useState('');
    const [login, setLogin] = useState('');
    const [pass, setPass] = useState('');
    const navigate = useNavigate();
    async function toSignUp(){
        try{
            const responce = await axios.post("https://localhost:7083/api/v1/UserPass/Registration", {
                email: email,
                login: login,
                password: pass
            });
            if (responce.status==200){
                navigate('/');
            }
        }catch(err){
            console.log(err);
        }
    }
    return (<>
        <div className={s.container}>
            <div className={s.form}>
                <img className={s.logo} src={icon_app} />
                <p className={s.title}>Регистрация</p>
                <Input type="text" alt="Email" onChange={setEmail}/>
                <Input type="text" alt="Логин" onChange={setLogin}/>
                <Input type="password" alt="Пароль" onChange={setPass}/>
                <div className={s.block}>
                    <Button text="Зарегиcтрироваться" onClick={toSignUp}/>
                    <Link title="Войти" path="/"/>
                </div>
            </div>
        </div>
    </>);
}

export default Registration;