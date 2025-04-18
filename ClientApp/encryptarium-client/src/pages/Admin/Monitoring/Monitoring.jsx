import { useEffect, useState } from "react"
import Input from "../../../components/Input/Input";
import s from "./style.module.css"
import Button from "../../../components/Button/Button";
import { bodyHistory } from "../../../entities/bodyHistory";
import { AuthorizePoliciesEnum, ControllersEnum, EntityEnum, MicroservicesEnum, StatusCodeEnum } from "../../../enums/Enums";
import { getAllRecordsForAdmin, getHistory } from "../../../utils/auditClient";
import toast from "react-hot-toast/headless";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faArrowDown, faArrowUp, faTriangleExclamation } from "@fortawesome/free-solid-svg-icons";

const Monitoring = () => {
    const [userUid, setUserUid] = useState("");
    const [microservice, setMicroservice] = useState(MicroservicesEnum.All);
    const [controller, setController] = useState(ControllersEnum.All);
    const [method, setMethod] = useState("");
    const [authorizePolicies, setAuthorizePolicies] = useState(AuthorizePoliciesEnum.All);
    const [statusCode, setStatusCode] = useState(StatusCodeEnum.All);
    const [dateStart, setDateStart] = useState(new Date().toISOString());
    const [dateEnd, setDateEnd] = useState(new Date().toISOString());
    const [entity, setEntity] = useState(EntityEnum.All);
    const [entityUid, setEntityUid] = useState("");
    const [history, setHistory] = useState(null);
    const [showHistory, setShowHistory] = useState(false);
    const [skip, setSkip] = useState(0);
    const [count, setCount] = useState(0);
    const [extra, setExtra] = useState(false);
    const [currentPage, setCurrentPage] = useState(0);
    const pattern = /^[0-9a-fA-F]{8}\b-[0-9a-fA-F]{4}\b-[0-9a-fA-F]{4}\b-[0-9a-fA-F]{4}\b-[0-9a-fA-F]{12}$/gi;
    async function getData(){
        bodyHistory.authorizePolicies = Number(authorizePolicies) === AuthorizePoliciesEnum.All ? null : Number(authorizePolicies);
        bodyHistory.userUid = userUid.match(pattern) ? userUid : null;
        bodyHistory.microservice = Number(microservice) === MicroservicesEnum.All ? null : Number(microservice);
        bodyHistory.controller = Number(controller) === ControllersEnum.All ? null : Number(controller);
        bodyHistory.method = method === "" ? null : method;
        bodyHistory.statusCode = Number(statusCode) === StatusCodeEnum.All ? null : Number(statusCode);
        bodyHistory.dateStart = new Date(dateStart);
        bodyHistory.dateEnd = new Date(dateEnd);
        bodyHistory.entity = Number(entity) === EntityEnum.All ? null : Number(entity);
        bodyHistory.entityUid = entityUid.match(pattern) ? entityUid : null;
        bodyHistory.skip = skip;
        const result = await getAllRecordsForAdmin({bodyHistory});
        if (result.isSuccess){
            setHistory(result.data);
            setCount(result.data.count)
            setCurrentPage(result.data.currentPage);
            console.log(result.data);
        }
        else {
            toast("Произошла непредвиденная ошибка",{
                duration: 4000,
                position: 'bottom-right',
                className: "notification_success",
                icon: <FontAwesomeIcon icon={faTriangleExclamation} style={{color: "#ff0000"}} />,
                removeDelay: 1000
            });
        }
        bodyHistory.authorizePolicies = AuthorizePoliciesEnum.All;
        bodyHistory.userUid = "";
        bodyHistory.microservice = MicroservicesEnum.All;
        bodyHistory.controller = ControllersEnum.All;
        bodyHistory.method = "";
        bodyHistory.statusCode = StatusCodeEnum.All;
        bodyHistory.dateStart = new Date().toISOString();
        bodyHistory.dateEnd = new Date().toISOString();
        bodyHistory.entity = EntityEnum.All;
        bodyHistory.entityUid = "";
    }
    function minusSkip(){
        if (skip <= 0){
            setSkip(0);
        }
        else{
            setSkip(skip - 10);
        }
    }
    function plusSkip(){
        if (skip / 10 >= count){
            setSkip(skip - 10);
        }
        else if ((skip + 10) / 10 == count){
            setSkip(skip);
        }
        else if (skip / 10 < count){
            setSkip(skip + 10);
        }
    }
    useEffect(() => {
        setShowHistory(false);
        getData();
    }, [skip])
    useEffect(() => {
        setShowHistory(false);
        getData();
    }, []);
    useEffect(() => {
        if (history !== null && history !== undefined){
            setShowHistory(true);
        } else setShowHistory(false);
    }, [history]);
    return (
        <div className={s.container_monitoring}>
            <div>
                <div className={s.monitoring_list}>
                    <Input style={{width:'20%', margin:'0'}} alt="Микросервис" type="text" list="Microservices" value={microservice} onChange={(e) => {setSkip(0); setMicroservice(e.target.value);}}/>
                    <Input style={{width:'20%', margin:'0'}} alt="Контроллер" type="text" list="Controllers" value={controller} onChange={(e) => {setSkip(0); setController(e.target.value)}}/>
                    <Input style={{width:'20%', margin:'0'}} alt="Политика авторизации" type="text" list="AuthorizePolicies" value={authorizePolicies} onChange={(e) => {setSkip(0); setAuthorizePolicies(e.target.value)}}/>
                    <Input style={{width:'20%', margin:'0'}} alt="Статус код" type="text" list="StatusCode" value={statusCode} onChange={(e) => {setSkip(0); setStatusCode(e.target.value)}}/>
                    <Input style={{width:'20%', margin:'0'}} alt="Сущность"type="text" list="Entities" value={entity} onChange={(e) => {setSkip(0); setEntity(e.target.value)}}/>
                </div>
                <div className={s.monitoring_date}>
                    <Input style={{margin:'0'}} alt="Дата начала" type="datetime-local" value={dateStart} onChange={(e) => {setSkip(0); setDateStart(e.target.value)}}/>
                    <Input style={{ margin:'0'}} alt="Дата конца" type="datetime-local" value={dateEnd} onChange={(e) => {setSkip(0); setDateEnd(e.target.value)}}/>
                    {!extra?<FontAwesomeIcon icon={faArrowDown} className={s.extraIcon} style={{color: "#282c34"}} onClick={()=>setExtra(true)} />
                        :<FontAwesomeIcon icon={faArrowUp} className={s.extraIcon} style={{color: "#282c34"}} onClick={()=>setExtra(false)} />}
                    <Button text="Поиск" onClick={() => {getData();}}/>
                </div>
                
                {extra&&<div className={s.monitoring_text}>
                    <Input alt="Uid пользователя" type="text" value={userUid} onChange={(e) => {setSkip(0); setUserUid(e.target.value)}}/>
                    <Input alt="Метод контроллера" type="text" value={method} onChange={(e) => {setSkip(0); setMethod(e.target.value)}}/>
                    <Input alt="Uid сущности" type="text" value={entityUid} onChange={(e) => {setSkip(0); setEntityUid(e.target.value)}}/>
                    
                </div>}
                
            </div>
            
            <div className={s.table_scroll}>
                <table className={s.monitoring_table}>
                    <thead className={s.table_head}>
                        <tr>
                            <th className={s.monitoring_th} scope="col">Микросервис</th>
                            <th className={s.monitoring_th} scope="col">Контроллер</th>
                            <th className={s.monitoring_th} scope="col">Политика</th>
                            <th className={s.monitoring_th} scope="col">Статус</th>
                            <th className={s.monitoring_th} scope="col">Сущность</th>
                            <th className={s.monitoring_th} scope="col">Дата</th>
                            <th className={s.monitoring_th} scope="col">Нужны были ли админские права?</th>
                            <th className={s.monitoring_th} scope="col">Uid пользователя</th>
                            <th className={s.monitoring_th} scope="col">Метод</th>
                            <th className={s.monitoring_th} scope="col">Uid сущности</th>
                        </tr>
                    </thead>
                    <tbody >
                        { showHistory ?
                            history.audits.map((h, index) => {
                                return(
                                <tr>
                                    <td className={s.monitoring_td}>{Object.keys(MicroservicesEnum)[h.sourceMicroservice]}</td>
                                    <td className={s.monitoring_td}>{Object.keys(ControllersEnum)[h.sourceController]}</td>
                                    <td className={s.monitoring_td}>{Object.keys(AuthorizePoliciesEnum)[h.authorizePolice]}</td>
                                    <td className={s.monitoring_td}>{Object.keys(StatusCodeEnum)[h.statusCode]}</td>
                                    <td className={s.monitoring_td}>{Object.keys(EntityEnum)[h.entity]}</td>
                                    <td className={s.monitoring_td}>{h.dateAct}</td>
                                    <td className={s.monitoring_td}>{String(h.isSourceRightAdmin)}</td>
                                    <td className={s.monitoring_td}>{h.userUid}</td>
                                    <td className={s.monitoring_td}>{h.sourceMethod}</td>
                                    <td className={s.monitoring_td}>{h.entityUid}</td>
                                </tr>
                                )
                            }) 
                            : <></>
                        }
                    </tbody>
                </table>
                
            </div>
            <div className={s.table_footer}>
                    <div>Всего страниц: {count}</div>
                    <Button text="<" classStyle="small" onClick={minusSkip}/>
                    {
                        <>{currentPage}</>
                    }
                    <Button text=">" classStyle="small" onClick={plusSkip}/>
                </div>
        </div>
    )
}

export default Monitoring;