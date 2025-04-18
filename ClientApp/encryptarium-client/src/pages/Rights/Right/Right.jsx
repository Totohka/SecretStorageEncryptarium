import { useState } from 'react';
import s from './style.module.css';

const Right = ({value}) => {
    const [isCreate, setCreate] = useState(value.item2.isCreate);
    const [isRead, setRead] = useState(value.item2.isRead);
    const [isUpdate, setUpdate] = useState(value.item2.isUpdate);
    const [isDelete, setDelete] = useState(value.item2.isDelete);
    return(
    <div className={s.module_container}>
        <div>
            <p>Uid: {value.item1.uid}</p>
            <p>Название: {value.item1.name}</p>
            <p>Описание: {value.item1.description}</p>
            <p>Код типа роли: {value.item1.codeType}</p>
            <p>Активна ли роль: {String(value.item1.isActive)}</p>
        </div>
        <div className={s.module_right}>
            <p>Создание секретов: <input type='checkbox' checked={isCreate} onChange={() => setCreate(!isCreate)}/></p>
            <p>Чтение: <input type='checkbox' checked={isRead} onChange={() => setRead(!isRead)}/></p>
            <p>Изменение: <input type='checkbox' checked={isUpdate} onChange={() => setUpdate(!isUpdate)}/></p>
            <p>Удаление: <input type='checkbox' checked={isDelete} onChange={() => setDelete(!isDelete)}/></p>
        </div>
    </div>
    );
}

export default Right;