import analytics from "./components/analytics";

/**
 * Called once the page is loaded and handles initialising
 * the different components.
 */
const init = () => {
    analytics();
};

const canInit = () => {
    const regReady = (window as any).attachEvent ? /d$|^c/ : /d$|^c|^i/; // eslint-disable-line @typescript-eslint/no-explicit-any
    return regReady.test(document.readyState || "");
};

let timer: NodeJS.Timeout;

const checkCanInit = () => {
    if (canInit()) {
        if (timer) {
            clearTimeout(timer);
        }

        init();
        return;
    }

    timer = setTimeout(checkCanInit, 100);
};

checkCanInit();