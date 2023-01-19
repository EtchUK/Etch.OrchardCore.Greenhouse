import getTrackingData from '../utils/getTrackingData';
import { v4 as uuidv4 } from 'uuid';

export default ($el: Element) => {
    $el.addEventListener('click', () => {
        if (!window.google_tag_manager) {
            return true;
        }

        // Google's recommended clearance of ecommerce before sending new data
        window.dataLayer.push({ ecommerce: null });

        // GA4
        window.dataLayer.push({
            event: "purchase",
            ecommerce: {
                transaction_id: uuidv4(),
                affiliation: window.location.hostname,
                value: 0,
                tax: 0,
                shipping: 0,
                currency: "USD",
                coupon: "",
                items: [getTrackingData($el)],
            },
            eventCallback: () => {
                const href = $el.getAttribute("href");

                if (href) {
                    window.open(href, "_blank");
                }
            },
            eventTimeout: 1500,
        });

        return false;
    });
};
