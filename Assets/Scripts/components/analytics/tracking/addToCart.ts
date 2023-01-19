import getTrackingData from '../utils/getTrackingData';

export default ($el: Element) => {
    const $inputElements = $el.querySelectorAll('input, select, textarea');
    let hasTracked = false;

    const track = () => {
        if (!window.google_tag_manager) {
            return true;
        }

        if (hasTracked) {
            return;
        }

        hasTracked = true;

        // Google's recommended clearance of ecommerce before sending new data
        window.dataLayer.push({ ecommerce: null });

        // GA4
        window.dataLayer.push({
            event: 'add_to_cart',
            ecommerce: {
                items: [getTrackingData($el)],
            },
        });

        $inputElements.forEach(($input) => {
            $input.removeEventListener('input', track);
        });
    };

    $inputElements.forEach(($input) => {
        $input.addEventListener('input', track);
    });
};
