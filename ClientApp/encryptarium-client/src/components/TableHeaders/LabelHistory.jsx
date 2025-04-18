import s from "./style.module.css"

const LabelHistory = ({title, value}) =>{
    return(
        <div className={s.container}>
            <label className={s.label}>{title}</label>
            <div>{value}</div>            
        </div>
    );
}
export default LabelHistory;