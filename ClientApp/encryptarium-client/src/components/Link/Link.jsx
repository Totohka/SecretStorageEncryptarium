import { NavLink } from "react-router";
import s from "./style.module.css";
const Link = ({path, title}) => {
    return (
        <NavLink className={s.link} to={path}>{title}</NavLink>
    );
}
export default Link;