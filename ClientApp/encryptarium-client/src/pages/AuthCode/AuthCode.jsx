import s from "./style.module.css";
import Button from "../../components/Button/Button";
import icon_app from "../../assets/icons/IconApp.svg";
import Link from "../../components/Link/Link";
import { useEffect, useState } from "react";
import { useNavigate } from "react-router";
import axios from "axios";
import { toSendCode } from "../../utils/authClient";
const AuthCode = ({login, pass}) => {
    const [finishTime, setFinish] = useState(600);
    const [code, setCode] = useState('');
    const [[diffM, diffS], setDiff] = useState([0, 0]);
    const [tick, setTick] = useState(false);
    const navigate = useNavigate();
    async function toAuth(){
        const result = await toSendCode({login, pass, code});
        if (result.isSuccess) navigate('/safes');
        else console.log(result.error);
        // try{
        //     const responce = await axios.post(
        //         "https://localhost:7083/api/v1/UserPass/VerifyCode", 
        //         // "http://localhost:5146/api/v1/UserPass/VerifyCode", 
        //         {
        //         login: login,
        //         password: pass,
        //         code: code
        //     });
        //     console.log(responce);
        //     if (responce.status==200){
        //         navigate('/safes');
        //     }
        // }catch(err){
        //     console.log(err);
        // }
    }
    useEffect(()=> {
      setFinish(finishTime - 1);
      if (finishTime < 0) navigate("/") // время вышло
      setDiff([
        Math.floor((finishTime / 60) % 60), 
        Math.floor(finishTime % 60)
      ]) 
    }, [tick])
        
    useEffect(()=>{
      const timerID = setInterval(() => setTick(!tick), 1000);
      return () => clearInterval(timerID);
    }, [tick])
    return(
        <div className={s.container}>
            <div className={s.form}>
                <img className={s.logo} src={icon_app} />
                <p className={s.title}>Введите код из письма</p>
                <input className={s.input} type="text" onChange={(e)=>setCode(e.target.value)}/>
                <p className={s.label}>Оставшееся время: {diffM}:{diffS<10?`0${diffS}`:diffS}</p>
                <div className={s.block}>
                    <Button text="Подтвердить" onClick={toAuth}/>
                    <Link title="Вернуться к авторизации" path="/"/>
                </div>
            </div>
        </div>
    );
}
export default AuthCode;