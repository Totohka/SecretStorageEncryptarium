import { faCircleCheck, faPen, faPlus, faToggleOn, faTrashCan, faTriangleCircleSquare, faTriangleExclamation } from "@fortawesome/free-solid-svg-icons";
import Button from "../../../components/Button/Button";
import s from "./style.module.css"
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { useEffect, useState } from "react";
import { getAllForAdmin, getUsers } from "../../../utils/storageClient";
import { bodyUsers } from "../../../entities/bodyUsers";
import cx from 'classnames'
import toast from "react-hot-toast/headless";
import { deactivedApiKeysByUid, deactivedIpByUid, getApiKeysByUserUid, getIpsByApiKeyUid } from "../../../utils/authClient";
import { Switch } from "../../../components/Switch";


const ApiKey = () => {
    const [users, setUsers] = useState(null);
    const [ips, setIps] = useState(null);
    const [keys, setKeys] = useState(null);
    const [showUsers, setShowUsers] = useState(false);
    const [showKeys, setShowKeys] = useState(false);
    const [showIps, setShowIps] = useState(false);
    const [user, setUser] = useState(-1);
    const [keyId, setKeyId] = useState(-1);
    const [ipId, setIpId] = useState(-1);
    function handleToggle(e, index){
        setUser(index);
        setKeyId(-1);
        setIpId(-1);
        setShowIps(false);
        setIps(null);
    }
    function handleToggleKey(e, index){
        setKeyId(index);
        setIpId(-1);
    }
    function handleToggleIp(e, index){
        setIpId(index);
    }
    async function changeActiveKey(index){
        const result = await deactivedApiKeysByUid(keys[index].uid, false);
        if (result.isSuccess){
            let keysNew = keys;
            keysNew[index].isActive = !keysNew[index].isActive;
            setKeys(keysNew);
            toast("Успешно изменено",{
                duration: 4000,
                position: 'bottom-right',
                className: "notification_success",
                icon: <FontAwesomeIcon icon={faCircleCheck} style={{color: "#00ff04"}} />,
                removeDelay: 1000
            });
        }
        else {
            toast("Произошла непредвиденная ошибка",{
                duration: 4000,
                position: 'bottom-right',
                className: "notification_warning",
                icon: <FontAwesomeIcon icon={faTriangleExclamation} style={{color: "#ff0000"}} />,
                removeDelay: 1000
            });
        }
    }
    async function changeActiveIp(index) {
        const result = await deactivedIpByUid(ips[index].uid);
        if (result.isSuccess){
            let ipsNew = ips;
            ipsNew[index].isActive = !ipsNew[index].isActive;
            setIps(ipsNew);
            toast("Успешно изменено",{
                duration: 4000,
                position: 'bottom-right',
                className: "notification_success",
                icon: <FontAwesomeIcon icon={faCircleCheck} style={{color: "#00ff04"}} />,
                removeDelay: 1000
            });
        }
        else {
            toast("Произошла непредвиденная ошибка",{
                duration: 4000,
                position: 'bottom-right',
                className: "notification_warning",
                icon: <FontAwesomeIcon icon={faTriangleExclamation} style={{color: "#ff0000"}} />,
                removeDelay: 1000
            });
        }
    }
    async function getDataIps(){
        const result = await getIpsByApiKeyUid(keys[keyId].uid);
        if (result.isSuccess){
            console.log(result.data);
            setIps(result.data.data);
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
    }
    async function getDataApiKeys(){
        const result = await getApiKeysByUserUid(users[user].uid);
        if (result.isSuccess){
            setKeys(result.data.data);
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
    }
    async function getDataUsers(){
        const result = await getAllForAdmin(bodyUsers);
        if (result.isSuccess){
            setUsers(result.data.users);
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
    }
    useEffect(() => {
        if (user != -1 && users !== null && users !== undefined)
            getDataApiKeys();
    }, [user]);
    useEffect(() => {
        if (keyId != -1 && keys !== null && keys !== undefined)
            getDataIps();
    }, [keyId]);
    useEffect(() => {
        getDataUsers();
    }, [])
    useEffect(() => {
        setShowUsers(false);
        if (users !== undefined && users !== null){
            setShowUsers(true);
        } else setShowUsers(false);
    }, [users])
    useEffect(() => {
        setShowKeys(false);
        if (keys !== undefined && keys !== null){
            setShowKeys(true);
        } else setShowKeys(false);
    }, [keys])
    useEffect(() => {
        setShowIps(false);
        if (ips !== undefined && ips !== null){
            setShowIps(true);
        } else setShowIps(false);
    }, [ips])
    return (
        <div className={s.main_container_key}>
            <div className={s.column_user_key}>
                <div className={s.title_key}>Пользователи</div>
                { showUsers ? users.map((u, index) => {
                    return <div className={cx(s.cell_key, user==index&&s.active_cell_key)} onClick={(e) => handleToggle(e, index)}>{u.login} ({u.uid})</div>
                })
                : <>Загрузка...</>
                }
            </div>  
            <div className={s.column_api_key}>
                <div className={s.title_key}>API-ключи <FontAwesomeIcon icon={faPlus} /></div>
                {
                    showKeys ? keys.map((k, index) => {
                        return (
                            <div className={cx(s.api_cell_key, keyId==index&&s.active_cell_key)} onClick={(e) => handleToggleKey(e, index)}>
                                <div className={cx(s.api_cell, s.colored)}>
                                    <div className={s.api_cell_text}>{k.name}</div>
                                    <FontAwesomeIcon className={s.api_cell_icon} icon={faPen} style={{color: "#282c34"}} />
                                </div>
                                <div className={s.api_cell}>
                                    <div className={s.api_cell_text}>{k.keyHash}</div>
                                    <Switch key={`keys-${k.uid}`} check={k.isActive} id={`keys-${k.uid}`} onChange={()=>{changeActiveKey(index)}}/>
                                    <FontAwesomeIcon className={s.api_cell_icon} icon={faTrashCan} style={{color: "#282c34", marginLeft:'5px'}} />
                                </div>
                            </div>
                        )
                    })
                    :
                    <></>
                }
            </div>  
            <div className={s.column_ip_key}>
                <div className={s.title_key}>IP-адреса <FontAwesomeIcon icon={faPlus} onClick={() => {}}/></div>
                { showIps ?
                    ips.map((i, index) => {
                        return(
                            <div className={s.cell_key}>
                                <div className={s.api_cell_text}>{i.ip}</div>
                                <div style={{display:'flex'}}>
                                    <FontAwesomeIcon icon={faPen} style={{color: "#282c34", marginRight:'5px'}} />
                                    <Switch key={`ips-${i.uid}`} check={i.isActive} id={`ips-${i.uid}`} onChange={()=>{changeActiveIp(index)}}/>
                                    <FontAwesomeIcon icon={faTrashCan} style={{color: "#282c34", marginRight:'5px', marginLeft:'5px'}} />
                                </div>
                            </div>
                        )
                    })
                    : <></>
                }
            </div>    
        </div>
    );
}

export default ApiKey;