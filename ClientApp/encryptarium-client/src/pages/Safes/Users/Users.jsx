import { useEffect, useState } from "react";
import Input from "../../../components/Input/Input";
import { bodyUsers } from "../../../entities/bodyUsers";
import { getUsers, getUsersByStorageUid } from "../../../utils/storageClient";
import s from "./style.module.css";
import cx from 'classnames';
import { faArrowRotateLeft, faCircleCheck, faPlus, faTrashCan, faTriangleExclamation } from "@fortawesome/free-solid-svg-icons";
import Button from "../../../components/Button/Button";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { changeAccessGroupStorage } from "../../../utils/accessClient";
import toast from "react-hot-toast/headless";
const Users = ({uid}) => {
    const [search, setSearch] = useState("");
    const [user, setUser] = useState(null);
    const [isLoad, setIsLoad] = useState(false);
    const [users, setUsers] = useState([]);
    const [usersByStorage, setUsersByStorage] = useState([]);
    const [rights, setRights] = useState([]);
    const [defaultRights, setDefaultRights] = useState([]);
    const [invite, setInvite] = useState(false);
    const [right, setRight] = useState(null);
    const [showRight, setShowRight] = useState(false);
    async function getStorageUsers(){
        const result = await getUsersByStorageUid(uid)
        if (result.isSuccess){
            let newList = result.data.users;
            newList.forEach((element, index) => {
                element.isCreate = false;
                element.isDelete = false;
                element.isUpdate = false; 
            });
            setUsersByStorage(newList);
            result.data.rights.forEach((element, i) => {
                element.index = i
            });
            setRights([...result.data.rights]);
            
            const requestRights = structuredClone(result.data.rights);
            setDefaultRights(requestRights);
        }
        else console.log(result.error);
    }
    async function sendData() {
        let data = {
            create:new Map(),
            update:new Map(),
            delete:[]
        }
        usersByStorage.forEach((u, index)=>{
            if(u.isCreate){
                data.create = {...data.create, [u.uid]: rights[index]};
            }
            else if(u.isUpdate){
                data.update = {...data.update, [u.uid]: rights[index]};
            }
            else if(u.isDelete){
                data.delete = [...data.delete, u.uid];
            }
        });
        const result = await changeAccessGroupStorage(uid, data);
        if (result.isSuccess && result.data){
            toast("Успешно изменено",{
                duration: 4000,
                position: 'bottom-right',
                className: "notification_success",
                icon: <FontAwesomeIcon icon={faCircleCheck} style={{color: "#00ff04"}} />,
                removeDelay: 1000
            });

            // showRight = false;
            setShowRight(false);
            setDefaultRights(structuredClone(rights));
            let usersNew = usersByStorage;
            usersNew.forEach(user => {
                user.isCreate = false;
                user.isDelete = false;
                user.isUpdate = false;
            });
            setUsersByStorage(usersNew);
            setRight(null);
            setUser(null);
            // setShowRight(true);
            // showRight = true;
        }
        else {
            toast("Непредвиденная ошибка",{
                duration: 4000,
                position: 'bottom-right',
                className: "notification_warning",
                icon: <FontAwesomeIcon icon={faTriangleExclamation} style={{color: "#ff0000"}} />,
                removeDelay: 1000
            });
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
    function handleChangeInvite(index){
        if (usersByStorage[index].isDelete && !usersByStorage[index].isUpdate && !usersByStorage[index].isCreate){
            let users = usersByStorage;
            users[index].isDelete = !users[index].isDelete;
            setUsersByStorage([...users]);
        }
        else if (!usersByStorage[index].isDelete && usersByStorage[index].isUpdate && !usersByStorage[index].isCreate){
            let users = usersByStorage;
            users[index].isUpdate = false;
            setUsersByStorage([...users]);
            let oldRights = rights;
            oldRights[index] = defaultRights[index];
            setShowRight(false);
            setRights(oldRights);
            setRight(defaultRights[index]);
        }
        else if (!usersByStorage[index].isDelete && !usersByStorage[index].isUpdate && usersByStorage[index].isCreate){
            setUsersByStorage(oldvalue => oldvalue.filter((i, key) => key !== index));
            setRights(oldvalue => oldvalue.filter((i, key) => key !== index));
        }
        else if (!usersByStorage[index].isDelete && !usersByStorage[index].isUpdate && !usersByStorage[index].isCreate){
            let users = usersByStorage;
            users[index].isDelete = !users[index].isDelete;
            setUsersByStorage([...users]);
        }
    }
    function handleChangeRightIsGroup(is){
        let oldRights = rights;
        oldRights[right.index].isGroup = is;
        if(is){
            oldRights[right.index]={isGroup: true, isRead: true, isCreate: false, isUpdate: false, isDelete: false, index:right.index};
            setShowRight(false);
        }
        if (!usersByStorage[right.index].isCreate && usersByStorage[right.index].isUpdate && !usersByStorage[right.index].isDelete){
            let defaultRight = defaultRights[right.index];
            if (defaultRight.isGroup === is)
            {
                let users = usersByStorage;
                users[right.index].isUpdate = false;
                setUsersByStorage([...users]);
            }
        }
        else if (!usersByStorage[right.index].isCreate && !usersByStorage[right.index].isUpdate && !usersByStorage[right.index].isDelete){
            let users = usersByStorage;
            users[right.index].isUpdate = true;
            setUsersByStorage([...users]);
        }
        setRights([...oldRights]);
        setRight(oldRights[right.index]);
    }
    function handleChangeRightIsRead(is){
        let oldRights = rights;
        oldRights[right.index].isRead = is;
        setRights(oldRights);
        setRight({...oldRights[right.index], index: right.index});

        if (!usersByStorage[right.index].isCreate && usersByStorage[right.index].isUpdate && !usersByStorage[right.index].isDelete){
            let defaultRight = defaultRights[right.index];
            
            if (defaultRight.isGroup === right.isGroup &&
                defaultRight.isCreate === right.isCreate &&
                defaultRight.isUpdate === right.isUpdate &&
                defaultRight.isRead === is &&
                defaultRight.isDelete === right.isDelete
            )
            {
                let users = usersByStorage;
                users[right.index].isUpdate = false;
                setUsersByStorage([...users]);
            }
        }
        else if (!usersByStorage[right.index].isCreate && !usersByStorage[right.index].isUpdate && !usersByStorage[right.index].isDelete){
            let users = usersByStorage;
            users[right.index].isUpdate = true;
            setUsersByStorage([...users]);
        }
    }
    function handleChangeRightIsCreate(is){
        let oldRights = rights;
        oldRights[right.index].isCreate = is;
        setRights(oldRights);
        setRight({...oldRights[right.index], index: right.index});

        if (!usersByStorage[right.index].isCreate && usersByStorage[right.index].isUpdate && !usersByStorage[right.index].isDelete){
            let defaultRight = defaultRights[right.index];
            console.log(defaultRight)
            if (defaultRight.isGroup === right.isGroup &&
                defaultRight.isCreate === is &&
                defaultRight.isUpdate === right.isUpdate &&
                defaultRight.isRead === right.isRead &&
                defaultRight.isDelete === right.isDelete
            )
            {
                let users = usersByStorage;
                users[right.index].isUpdate = false;
                setUsersByStorage([...users]);
            }
        }
        else if (!usersByStorage[right.index].isCreate && !usersByStorage[right.index].isUpdate && !usersByStorage[right.index].isDelete){
            let users = usersByStorage;
            users[right.index].isUpdate = true;
            setUsersByStorage([...users]);
        }
    }
    function handleChangeRightIsUpdate(is){

        let oldRights = rights;
        oldRights[right.index].isUpdate = is;
        setRights(oldRights);
        setRight({...oldRights[right.index], index: right.index});

        if (!usersByStorage[right.index].isCreate && usersByStorage[right.index].isUpdate && !usersByStorage[right.index].isDelete){
            let defaultRight = defaultRights[right.index];
            if (defaultRight.isGroup === right.isGroup &&
                defaultRight.isCreate === right.isCreate &&
                defaultRight.isUpdate === is &&
                defaultRight.isRead === right.isRead &&
                defaultRight.isDelete === right.isDelete
            )
            {
                let users = usersByStorage;
                users[right.index].isUpdate = false;
                setUsersByStorage([...users]);
            }
        }
        else if (!usersByStorage[right.index].isCreate && !usersByStorage[right.index].isUpdate && !usersByStorage[right.index].isDelete){
            let users = usersByStorage;
            users[right.index].isUpdate = true;
            setUsersByStorage([...users]);
        }

    }
    function handleChangeRightIsDelete(is){
        let oldRights = rights;
        oldRights[right.index].isDelete = is;
        setRights(oldRights);
        setRight({...oldRights[right.index], index: right.index});

        if (!usersByStorage[right.index].isCreate && usersByStorage[right.index].isUpdate && !usersByStorage[right.index].isDelete){
            let defaultRight = defaultRights[right.index];
            if (defaultRight.isGroup === right.isGroup &&
                defaultRight.isCreate === right.isCreate &&
                defaultRight.isUpdate === right.isUpdate &&
                defaultRight.isRead === right.isRead &&
                defaultRight.isDelete === is
            )
            {
                let users = usersByStorage;
                users[right.index].isUpdate = false;
                setUsersByStorage([...users]);
            }
        }
        else if (!usersByStorage[right.index].isCreate && !usersByStorage[right.index].isUpdate && !usersByStorage[right.index].isDelete){
            let users = usersByStorage;
            users[right.index].isUpdate = true;
            setUsersByStorage([...users]);
        }
    }
    function handleReturnStyle(index){

        if (usersByStorage[index].isDelete && !usersByStorage[index].isUpdate && !usersByStorage[index].isCreate){
            return(s.delete_user);
        }
        else if (!usersByStorage[index].isDelete && usersByStorage[index].isUpdate && !usersByStorage[index].isCreate){
            return(s.change_user);
        }
        else if (!usersByStorage[index].isDelete && !usersByStorage[index].isUpdate && usersByStorage[index].isCreate){
            return(s.add_user);
        }
        else if (!usersByStorage[index].isDelete && !usersByStorage[index].isUpdate && !usersByStorage[index].isCreate){
            return("");
        }
        return("");
    }
    function handleReturnIcon(index){
        if (usersByStorage[index].isDelete && !usersByStorage[index].isUpdate && !usersByStorage[index].isCreate){
            return(<FontAwesomeIcon icon={faPlus}/>);
        }
        else if (!usersByStorage[index].isDelete && usersByStorage[index].isUpdate && !usersByStorage[index].isCreate){
            return(<FontAwesomeIcon icon={faArrowRotateLeft} />);
        }
        else if (!usersByStorage[index].isDelete && !usersByStorage[index].isUpdate && usersByStorage[index].isCreate){
            return(<FontAwesomeIcon icon={faTrashCan}/>);
        }
        else if (!usersByStorage[index].isDelete && !usersByStorage[index].isUpdate && !usersByStorage[index].isCreate){
            return(<FontAwesomeIcon icon={faTrashCan}/>);
        }
        return(<FontAwesomeIcon icon={faPlus}/>);
    }
    function handleInvite(item){
        if(usersByStorage.filter((i)=>i.uid==item.uid).length!=0) return;
        setInvite(false);
        item.isDelete = false;
        item.isCreate = true;
        item.isUpdate = false;
        setUsersByStorage([...usersByStorage, item]);
        setRights([...rights, {isGroup: true, isRead: true, isCreate: false, isUpdate: false, isDelete: false, index:rights.length}]);
    }
    function handleSeeRightUser(index, e){
        setShowRight(false);
        if(user==usersByStorage[index]){
            setUser(null)
            setRight(null);
        }
        else{
            setUser(usersByStorage[index]);
            setRight(rights[index]);
        }
        
    }
    useEffect(() =>{
        setShowRight(false);
        if (right !== null){
            // console.log(right);
            setShowRight(true);
        }
    }, [right])
    useEffect(() => {
        getStorageUsers();
    }, [])
    useEffect(()=>{
        setInvite(false);
        setInvite(true);
    },[usersByStorage])
    useEffect(()=>{
        setIsLoad(true);
    }, [users])
    useEffect(
    () => {
        setIsLoad(false);
        getSearchUsers();
    }, [search])
    return (
        <div>
            <div className={s.all_container}>
                <div className={s.column_search_users}>
                    <div>Доступные пользователей</div>
                    <Input alt="Поиск" onChange={(e) => {setSearch(e.target.value)}} />
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
                    <div>Пользователи в сейфе</div>
                    <div className={s.body_invite_user}>                  
                        {   
                            invite ?
                            usersByStorage.map((u, index) => {
                                return(
                                    <div className={s.cell_invite_users} onClick={(e) => handleSeeRightUser(index, e)}>
                                        <div className={s.user_and_marker}>
                                            <div className={cx(s.circle_marker, handleReturnStyle(index))}></div>
                                            {u.login}, email: {u.email}
                                        </div>
                                        <Button classStyle="small" 
                                            icon = {handleReturnIcon(index)}
                                            text="" 
                                            onClick={(e)=>{e.stopPropagation(); handleChangeInvite(index)}}/>
                                    </div>
                                )}
                            )
                            : <></>
                        }
                    </div>
                </div>
                <div className={s.column_right_invite_user}>
                    <div>Права</div>
                    <div className={s.block_rights}>
                    { showRight ?
                        <div>
                            <div className={s.bordered}>
                                <Input defaultChecked={Boolean(right.isGroup)} alt="Групповая роль" type="checkbox" onChange={(e) => {handleChangeRightIsGroup(e.target.checked)}} />
                            </div>
                            <Input defaultChecked={Boolean(right.isRead)} alt="Чтение" type="checkbox" onChange={(e) => {handleChangeRightIsRead(e.target.checked)}} active={!right.isGroup}/>
                            <Input defaultChecked={Boolean(right.isCreate)} alt="Создание" type="checkbox" onChange={(e) => {handleChangeRightIsCreate(e.target.checked)}}  active={!right.isGroup}/>
                            <Input defaultChecked={Boolean(right.isUpdate)} alt="Редактирование" type="checkbox" onChange={(e) => {handleChangeRightIsUpdate(e.target.checked)}}  active={!right.isGroup}/>
                            <Input defaultChecked={Boolean(right.isDelete)} alt="Удаление" type="checkbox" onChange={(e) => {handleChangeRightIsDelete(e.target.checked)}}  active={!right.isGroup}/>
                        </div>
                        : <></>
                    }
                    </div>
                </div>
            </div>
            <Button text="Сохранить" onClick={sendData}/>
        </div>
    );
}

export default Users;