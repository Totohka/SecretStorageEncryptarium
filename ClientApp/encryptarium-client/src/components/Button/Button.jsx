import s from "./style.module.css";
import parse from 'html-react-parser';
import cx from 'classnames';

const Button = ({text, onClick, classStyle, icon}) => {
    return(
        <button className={cx(s.button, s[classStyle])} onClick={onClick}>
            {icon}{parse(text)}
        </button>
    )
}
export default Button;