import getTrackingData from '../utils/getTrackingData';
import getListName from '../utils/getListName';

export default ($el: Element) => {
    $el.addEventListener('click', (e) => {
        if (!window.google_tag_manager) {
            return true;
        }

        e.preventDefault();

        // Google's recommended clearance of ecommerce before sending new data
        window.dataLayer.push({ ecommerce: null });

        // GA4
        const ga4Event = {
            event: "select_item",
            ecommerce: {
                items: [getTrackingData($el)],
            },
            eventCallback: () => {
                const href = $el.getAttribute("href");

                if (href) {
                    document.location.href = href;
                }
            },
            eventTimeout: 1500,
        };

        ga4Event.ecommerce.items[0].item_list_name = getListName($el);

        window.dataLayer.push(ga4Event);

        return false;
    });
};
