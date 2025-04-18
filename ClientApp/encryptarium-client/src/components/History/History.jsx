import s from './style.module.css'
import LabelHistory from '../TableHeaders/LabelHistory';
import Button from '../Button/Button';
import { useState } from 'react';
import { ControllersEnum, EntityEnum } from '../../enums/Enums';
const History = ({type, h, id}) => {
    const [show, setShow] = useState(false);
    return(
        <>
        { type === "human" ?
            <div className={s.auditSecretModule}>
                <LabelHistory title={"Дата и время: "} value={h.dateAct}></LabelHistory>
                <LabelHistory title={"Тип сущности: "} value={Object.keys(EntityEnum)[h.entity]}></LabelHistory>
                <LabelHistory title={"Uid сущности: "} value={!h.entityUid ? "Отсутствует" : h.entityUid}></LabelHistory>
                <LabelHistory title={"Контроллер: "} value={Object.keys(ControllersEnum)[h.sourceController]}></LabelHistory>
                <div className={s.container_button_last_element}>
                    <LabelHistory title={"Метод: "} value={h.sourceMethod}></LabelHistory>
                    <Button id={id} text='&#8744;' onClick={()=>{setShow(!show)}}/>
                </div>
                {show ? <div id={id} key={id} >{h.action}</div>:<></>}
            </div>
            :
            <div className={s.auditSecretModule}>
                <LabelHistory title={"Дата и время: "} value={h.dateAct}></LabelHistory>
                <LabelHistory title={"Uid пользователя: "} value={h.userUid}></LabelHistory>
                <div className={s.container_button_last_element}>
                    <LabelHistory title={"Метод: "} value={h.sourceMethod}></LabelHistory>
                    <Button id={id} text='&#8744;' onClick={()=>{setShow(!show)}}/>
                </div>
                {show ? <div id={id} key={id} >{h.action}</div>:<></>}
            </div>
        }
        </>

    );
}
export default History;