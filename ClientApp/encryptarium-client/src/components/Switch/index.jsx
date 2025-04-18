import s from './style.module.css';
export const Switch = ({id, onChange, check}) =>{
    return(
        <>
            <input defaultChecked={check} className={s.switch} type="checkbox" id={`switch-${id}`} onChange={onChange}/>
            <label className={s.switch_label} for={`switch-${id}`}>Toggle</label>
        </>
    )
}