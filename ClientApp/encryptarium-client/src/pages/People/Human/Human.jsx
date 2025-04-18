import { useEffect, useState } from "react";
import { getHistory } from "../../../utils/auditClient";
import { bodyHistory } from "../../../entities/bodyHistory";
import Input from "../../../components/Input/Input";
import Button from "../../../components/Button/Button";
import History from "../../../components/History/History";
import s from './style.module.css';
import edit from '../../../assets/icons/edit.svg'
import { StatusCodeEnum } from "../../../enums/Enums";

const Human = ({human}) => {
    const [history, setHistory] = useState(null);
    const [userUid, setUserUid] = useState(human.uid);
    const [microservice, setMicroservice] = useState(null); 
    const [controller, setController] = useState(null);
    const [method, setMethod] = useState(null);
    const [authorizePolicies, setAuthorizePolicies] = useState(null);
    const [statusCode, setStatusCode] = useState(null);
    const [dateStart, setDateStart] = useState((new Date(Date.now()-30*60*60*60*60*24)).toISOString());
    const [dateEnd, setDateEnd] = useState(new Date(Date.now()).toISOString());
    const [entity, setEntity] = useState(null);
    const [entityUid, setEntityUid] = useState(null);
    const [user, setUser] = useState(human);
    const [skip, setSkip] = useState(0);
    const [take, setTake] = useState(10);
    const [currentPage, setCurrentPage] = useState(1);
    const [countPage, setCountPage] = useState(1)
    const [loadHistory, setLoadHistory] = useState(true);
    const [load, setLoad] = useState(false);
    const [active, setActive] = useState(1);    
    async function getDataHistory(){
        bodyHistory.userUid = userUid;
        bodyHistory.dateStart = new Date(dateStart);
        bodyHistory.dateEnd = new Date(dateEnd);
        bodyHistory.entity = entity;
        bodyHistory.statusCode = Number(statusCode) === StatusCodeEnum.All ? null : Number(statusCode);
        bodyHistory.entityUid = null;
        const result = await getHistory({bodyHistory});
        if (result.isSuccess) {
            setHistory(result.data);
            setLoadHistory(false);
            setCurrentPage(result.data.currentPage);
        }
        else console.log(result.error);
    }
    function nextPage(){
        bodyHistory.skip += 10;
        setSkip(skip + 10);
    }
    function prevPage(){
        bodyHistory.skip -= 10;
        setSkip(skip - 10);
    }
    useEffect(()=>{
        getDataHistory();
    },[skip])
    useEffect(()=>{
        bodyHistory.userUid = userUid;
        bodyHistory.dateStart = dateStart;
        bodyHistory.dateEnd = dateEnd;
        bodyHistory.skip = skip;
        getDataHistory();
        }, [])
    return(
        <>
        {load?<>Загрузка...</>:
        <div className={s.container}>
            <div className={s.header}>{user.login}</div>
            <div className={s.buttons}>
                <button className={active==1?`${s.button} ${s.active}`:s.button} onClick={()=>{setActive(1)}}>Общие</button>
                <button className={active==2?`${s.button} ${s.active}`:s.button} onClick={()=>{setActive(2)}}>История действий</button>
            </div>
            {active==1?
            <div className={s.module}>
                <div className={s.module_container}>
                    <div className={s.module_header}>Uid</div>
                    <div className={s.module_value}>{user.uid}</div>
                </div>
                <div className={s.module_container}>
                    <div className={s.module_header}>Логин</div>
                    <div className={s.module_value}>{user.login}</div>
                </div>
                <div className={s.module_container}>
                    <div className={s.module_header}>Email</div>
                    <div className={s.module_value}>{user.email}</div>
                </div>
                <div className={s.module_container}>
                    <div className={s.module_header}>Дата создания</div>
                    <div className={s.module_value}>{user.dateCreate}</div>
                </div>
                <div className={s.module_container}>
                    <div className={s.module_header}>Админ</div>
                    <div className={s.module_value}>{user.isAdmin?'Да':'Нет'}</div>
                </div>
                <div className={s.module_container}>
                    <div className={s.module_header}>Может создавать хранилища</div>
                    <div className={s.module_value}>{user.isCreateStorage?'Да':'Нет'}</div>
                </div>
                <div className={s.module_container}>
                    <div className={s.module_header}>Может создавать групповые роли</div>
                    <div className={s.module_value}>{user.isCreateGroupRole?'Да':'Нет'}</div>
                </div>
                <div className={s.module_container}>
                    <div className={s.module_header}>Статус</div>
                    <div className={s.module_value}>{user.isActive?'Активна':'Отключена'}</div>
                </div>
            </div>   
            : 
            <div className={s.module}>
                    <div >
                            {loadHistory?<>Загрузка...</>:
                                <div className={s.modele_container_pagination}>
                                <div className={s.module_header}>История</div>
                                    {
                                        skip !== 0 ?
                                        <Button text='<' classStyle="small" onClick={() => {prevPage()}}></Button>
                                        :
                                        <></>
                                    }
                                    <div style={{margin:'0px 10px'}}>{currentPage}</div>
                                    { 
                                        currentPage !== history.count ? 
                                        <Button text='>' classStyle="small" onClick={() => {nextPage()}}></Button>
                                        :
                                        <></>
                                    }
                                    <div className={s.module_value}>
                                        <label>Всего страниц:</label>
                                        {history.count}
                                    </div>
                                </div>
                            }
                        <div className={s.module_container_filter}>
                            <div style={{display: "flex"}}>
                                <Input alt="StatusCode" type="text" list="StatusCode" onChange={(e) => {setStatusCode(e.target.value)}}/>
                                <Input alt="DateStart" type="datetime-local" onChange={(e) => {setDateStart(e.target.value)}}/>
                                <Input alt="DateEnd" type="datetime-local" onChange={(e) => {setDateEnd(e.target.value)}}/>
                                <Input alt="Entity" type="text" list="Entities" onChange={(e) => {setEntity(e.target.value)}}/>
                            </div>
                            <Button text="Поиск" onClick={()=>{setLoadHistory(true); getDataHistory();}}/>
                        </div>
                        <div style={{'overflow-y':'auto','height':'51vh'}}>
                            {loadHistory?<>Загрузка...</>:history.audits.map((h, index)=>{
                                return(
                                    <History type={'human'} h={h} id={index}/>
                                );
                            })}
                        </div>
                    </div>
                </div>
            }
        </div>
        }
        </>
    );
}

export default Human;