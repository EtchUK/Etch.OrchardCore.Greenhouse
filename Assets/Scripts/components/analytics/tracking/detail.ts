import getTrackingData from '../utils/getTrackingData';

export default ($el: Element) => {
    if (!window.google_tag_manager) {
        return;
    }

    // Google's recommended clearance of ecommerce before sending new data
    window.dataLayer.push({ ecommerce: null });

    // GA4
    window.dataLayer.push({
        event: 'view_item',
        ecommerce: {
            items: [getTrackingData($el)],
        },
    });
};
