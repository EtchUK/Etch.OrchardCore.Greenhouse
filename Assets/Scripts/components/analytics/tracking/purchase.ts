import getTrackingData from '../utils/getTrackingData';
import { v4 as uuidv4 } from 'uuid';

export default ($el: Element) => {
    const applicationId = uuidv4();

    // Google's recommended clearance of ecommerce before sending new data
    window.dataLayer.push({ ecommerce: null });

    // GA4
    window.dataLayer.push({
        event: 'purchase',
        ecommerce: {
            transaction_id: applicationId,
            affiliation: window.location.hostname,
            value: 0,
            tax: 0,
            shipping: 0,
            currency: 'USD',
            coupon: '',
            items: [getTrackingData($el)],
        },
    });
};
