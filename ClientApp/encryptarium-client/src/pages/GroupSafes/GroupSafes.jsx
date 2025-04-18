import { useEffect, useState } from "react";
import Input from "../../components/Input/Input";
import s from "./style.module.css"
import { bodyUsers } from "../../entities/bodyUsers";
import { getUsers } from "../../utils/storageClient";
import Button from "../../components/Button/Button";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faTrashCan } from "@fortawesome/free-solid-svg-icons";
import Safes from "../Safes/Safes";
import { TypeStorageEnum } from "../../enums/Enums";
import cx from "classnames";
import { createGroupRoleAndStorage } from "../../utils/accessClient";

const GroupSafes = () => {
    const [user, setUser] = useState(null);
    const [users, setUsers] = useState([]);
    const [inviteUsers, setInviteUsers] = useState([]);
    const [rightInviteUser, setRightInviteUser] = useState([]);
    const [right, setRight] = useState(null);
    const [showRight, setShowRight] = useState(false);
    const [search, setSearch] = useState("");
    const [oldE, setOldE] = useState();
    const [isLoad, setIsLoad] = useState(false);
    const [invite, setInvite] = useState(false);
    const [nameStorage, setNameStorage] = useState(""); 
    const [descriptionStorage, setDescriptionStorage] = useState(""); 
    const [nameRole, setNameRole] = useState(""); 
    const [descriptionRole, setDescriptionRole] = useState(""); 
    const [active, setActive] = useState(1);

    async function createSafe() {
        let data = {
            StorageTitle: nameStorage,
            StorageDescription: descriptionStorage,
            RoleName: nameRole,
            RoleDescription: descriptionRole,
            RightUsers:{}
        };
        inviteUsers.forEach((element, index) => {
            let setNew = Object;
            setNew[element.uid] = rightInviteUser[index];
            data.RightUsers = {...data.RightUsers, ...setNew};
        });
        const result = await createGroupRoleAndStorage(data);
        if (!result.isSuccess) {
            console.log(result.error);
        }
    }
    async function getSearchUsers() {
        if (search !== ""){
            bodyUsers.login = search;
            bodyUsers.email = null;
            const result = await getUsers(bodyUsers);
            if (result.isSuccess) {
                setUsers(result.data.users);
            }
            else console.log(result.error);
        }
        else{
            setUsers([]);
        }
    }
    function handleChangeRightIsGroup(is){
        let rights = rightInviteUser;
        rights[right.index].isGroup = is;
        if(is){
            rights[right.index]={isGroup: true, isRead: true, isCreate: false, isUpdate: false, isDelete: false};
            setShowRight(false);
        }
        setRightInviteUser(rights);
        
        setRight({...rights[right.index], index: right.index});
    }
    function handleChangeRightIsRead(is){
        let rights = rightInviteUser;
        rights[right.index].isRead = is;
        setRightInviteUser(rights);
        setRight({...rights[right.index], index: right.index});
    }
    function handleChangeRightIsCreate(is){
        let rights = rightInviteUser;
        rights[right.index].isCreate = is;
        setRightInviteUser(rights);
        setRight({...rights[right.index], index: right.index});
    }
    function handleChangeRightIsUpdate(is){
        let rights = rightInviteUser;
        rights[right.index].isUpdate = is;
        setRightInviteUser(rights);
        setRight({...rights[right.index], index: right.index});
    }
    function handleChangeRightIsDelete(is){
        let rights = rightInviteUser;
        rights[right.index].isDelete = is;
        setRightInviteUser(rights);
        setRight({...rights[right.index], index: right.index});
    }
    function handleSeeRightUser(indexItem, e){
        setShowRight(false);
        oldE?.target.classList.remove(s.taken);
        e.target.classList.add(s.taken);
        setOldE(e);
        setRight({...rightInviteUser[indexItem], index: indexItem});
        setUser(inviteUsers[indexItem]);
    }
    function handleUninvite(indexItem){
        setShowRight(false);
        setRight(null); 
        setUser(null); 
        setInviteUsers((prevUsers) => prevUsers.filter((i, index) => index !== indexItem));
        setRightInviteUser((prevRight) => prevRight.filter((i, index) => index !== indexItem));
    }
    function handleInvite(user){
        if (inviteUsers.find(u => u.uid === user.uid) === undefined){
            setInviteUsers([...inviteUsers, user]);
            setRightInviteUser([...rightInviteUser, {isGroup: true, isRead: true, isCreate: false, isUpdate: false, isDelete: false}]);
            console.log(rightInviteUser);
        }
    }
    useEffect(() => {
        if (right === null){
            setShowRight(false);
        }else{
            setShowRight(true);
        }
    }
    , [right])
    useEffect(() => {
            setInvite(false);
            setInvite(true);
        }
    , [inviteUsers])
    useEffect(
        () => {
            setIsLoad(true);
        }
    , [users])
    useEffect(
        () =>{
            setIsLoad(false);
            getSearchUsers();
        }
    , [search])
    return(
        <>
            <div className={s.buttons}>
                <button className={active==1?`${s.button} ${s.active}`:s.button} onClick={()=>{setActive(1)}}>Создание</button>
                <button className={active==2?`${s.button} ${s.active}`:s.button} onClick={()=>{setActive(2)}}>Управление</button>
            </div>
            <div className={s.all_container}>
                { active === 1?
                <>
                    <div className={s.container_component}>
                        
                        <div className={s.column_main_settings}>
                            <div>Общие настройки</div>
                            <Input alt="Название сейфа" onChange={(e)=>setNameStorage(e.target.value)}/>
                            <Input alt="Описание сейфа" onChange={(e)=>setDescriptionStorage(e.target.value)}/>
                            <Input alt="Название роли" onChange={(e)=>setNameRole(e.target.value)}/>
                            <Input alt="Описание роли" onChange={(e)=>setDescriptionRole(e.target.value)}/>
                        </div>
                        <div className={s.column_search_users}>
                            <div>Доступные пользователи</div>
                            <Input alt="Поиск" onChange={(e)=>setSearch(e.target.value)}/>
                            <div className={s.body_search_users}>
                                {   
                                    isLoad ?
                                        users.map((u) => {
                                            return(
                                                <div className={cx(s.cell_search_users, user!==null&&u.email==user.email?s.taken:'')} onClick={()=>handleInvite(u)}>
                                                    <p>{u.login}</p>
                                                    <p>email: {u.email}</p>
                                                </div>
                                            );
                                        })
                                    : <></>
                                }
                            </div>
                        </div>
                        <div className={s.column_invite_user}>
                            <div>Приглашённые пользователи</div>
                            <div className={s.body_invite_user}>
                                {   
                                    invite ?
                                    inviteUsers.map((u, index) => {
                                        return(
                                            <div  className={cx(s.cell_invite_users)} onClick={(e) => handleSeeRightUser(index, e)}>
                                                {u.login}, email: {u.email}
                                                <Button classStyle="small" icon={<FontAwesomeIcon icon={faTrashCan} />} text="" onClick={(e)=>{e.stopPropagation(); handleUninvite(index);}}/>
                                            </div>
                                        )}
                                    )
                                    : <></>
                                }
                            </div>
                        </div>
                        <div className={s.column_right_invite_user}>
                            <div>Права пользователей</div>
                            <div className={s.body_invite_user}>
                                {
                                    showRight ?
                                    <div>
                                        <div className={s.header_rights}>Права для пользователя {user.login}</div>
                                        <div  className={s.block_rights}>
                                            <div className={s.bordered}>
                                                <Input defaultChecked={Boolean(right.isGroup)} alt="Групповая роль" type="checkbox" onChange={(e) => {handleChangeRightIsGroup(e.target.checked)}} />
                                            </div>
                                            <Input defaultChecked={Boolean(right.isRead)} alt="Чтение" type="checkbox" onChange={(e) => {handleChangeRightIsRead(e.target.checked)}} active={!right.isGroup}/>
                                            <Input defaultChecked={Boolean(right.isCreate)} alt="Создание секретов" type="checkbox" onChange={(e) => {handleChangeRightIsCreate(e.target.checked)}}  active={!right.isGroup}/>
                                            <Input defaultChecked={Boolean(right.isUpdate)} alt="Редактирование" type="checkbox" onChange={(e) => {handleChangeRightIsUpdate(e.target.checked)}}  active={!right.isGroup}/>
                                            <Input defaultChecked={Boolean(right.isDelete)} alt="Удаление" type="checkbox" onChange={(e) => {handleChangeRightIsDelete(e.target.checked)}}  active={!right.isGroup}/>
                                        </div>
                                        <Button text="Скрыть" onClick={() => {setShowRight(false);}}/>
                                    </div>
                                    : <></>
                                }
                            </div>
                        </div>
                    </div>
                    <div className={s.button_container}>
                        <Button text="Создать сейф" onClick={createSafe}/>
                    </div>
                </>
                :
                <>
                    <Safes code={TypeStorageEnum.Group} />
                </>
                }
            </div>
        </>
    );
}

export default GroupSafes;