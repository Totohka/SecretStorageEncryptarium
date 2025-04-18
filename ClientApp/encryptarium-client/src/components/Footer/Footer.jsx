import './Footer.css';
import logo from '../../assets/icons/IconApp.svg';

const Footer = () => {
    return(
        <div className="footer_div">
            <img className="footer_logo" src={logo}/> @ Encryptarium Vault
        </div>
    );
}

export default Footer;