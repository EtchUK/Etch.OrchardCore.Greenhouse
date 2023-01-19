import getTrackingData from '../utils/getTrackingData';
import getListName from '../utils/getListName';

const track = ($el: Element) => {
    if (!window.google_tag_manager) {
        return;
    }

    // Google's recommended clearance of ecommerce before sending new data
    window.dataLayer.push({ ecommerce: null });

    // GA4
    const ga4Event = {
        event: 'view_item_list',
        ecommerce: {
            items: [getTrackingData($el)],
        },
    };

    ga4Event.ecommerce.items[0].item_list_name = getListName($el);

    window.dataLayer.push(ga4Event);
};

export default ($el: Element) => {
    if (!('IntersectionObserver' in window)) {
        track($el);
        return;
    }

    const observer = new IntersectionObserver(
        (entries) => {
            if (entries[0].isIntersecting) {
                track($el);
                observer.unobserve($el);
            }
        },
        { threshold: [0] }
    );

    observer.observe($el);
};
