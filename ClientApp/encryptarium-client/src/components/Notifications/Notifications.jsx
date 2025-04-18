import toast, { useToaster } from "react-hot-toast/headless";
import s from "./style.module.css"

const Notifications = () => {
    const { toasts, handlers } = useToaster();
    const { startPause, endPause, calculateOffset, updateHeight } = handlers;
    return (
      <div
        style={{
          display: "flex",
          flexDirection: "column",
          position: "fixed",
          bottom: 256,
          right: 260,
          zIndex: 100,
        }}
        onMouseEnter={startPause}
        onMouseLeave={endPause}
      >
        {toasts.map((toast) => {
          const offset = calculateOffset(toast, {
            reverseOrder: false,
            margin: 8
          });
          const ref = (el) => {
            if (el && typeof toast.height !== "number") {
              const height = el.getBoundingClientRect().height;
              updateHeight(toast.id, height);
            }
          };

          return (
            <div
              key={toast.id}
              ref={ref}
              className={s[toast.className]}
              style={{
                opacity: toast.visible ? 1 : 0,
                textAlign: "left",
                transform: `translateY(${offset}px)`
                // transform: `translateX(${offset}px)`
              }}
            >
              <div className={s.icon}>
                {toast.icon}
              </div>
                {toast.message}
            </div>
          );
        })}
      </div>
    );
  };

  export default Notifications;