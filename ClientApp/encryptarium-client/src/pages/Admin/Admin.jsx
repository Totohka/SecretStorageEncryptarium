import { useState } from "react";
import s from "./style.module.css"
;
import Monitoring from "./Monitoring/Monitoring";
import ApiKey from "./ApiKey/ApiKey";
import UserTokens from "./UserTokens/UserTokens";
const Admin = () => {
    const [active, setActive] = useState(1);
    function renderPage(){
        switch(active){
            case 1:
                return <UserTokens/>
            case 2:
                return <>2</>
            case 3:
                return <ApiKey />
            case 4:
                return <Monitoring />
            case 5:
                return <>5</>
        }
    }
    return(
    <div className={s.main_container}>
        <div className={s.buttons}>
            <button className={active==1?`${s.button} ${s.active}`:s.button} onClick={()=>{setActive(1)}}>Пользователи</button>
            <button className={active==2?`${s.button} ${s.active}`:s.button} onClick={()=>{setActive(2)}}>Уведомления</button>
            <button className={active==3?`${s.button} ${s.active}`:s.button} onClick={()=>{setActive(3)}}>API-ключи</button>
            <button className={active==4?`${s.button} ${s.active}`:s.button} onClick={()=>{setActive(4)}}>Мониторинг</button>
            <button className={active==5?`${s.button} ${s.active}`:s.button} onClick={()=>{setActive(5)}}>Доступы</button>
        </div>
        {
            renderPage()
        }
    </div>
    );
}

export default Admin;