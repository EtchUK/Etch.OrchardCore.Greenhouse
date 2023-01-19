/**
 * Custom analytics tracking.
 */

import addToCart from './tracking/addToCart';
import click from './tracking/click';
import detail from './tracking/detail';
import impression from './tracking/impression';
import purchase from './tracking/purchase';
import purchaseClick from './tracking/purchaseClick';

declare global {
    interface Window {
        dataLayer: any[]; // eslint-disable-line @typescript-eslint/no-explicit-any
        google_tag_manager: any; // eslint-disable-line @typescript-eslint/no-explicit-any
        gtag: Gtag.Gtag;
    }
}

const getEvents = ($el: Element) => {
    return $el
        .getAttribute('data-events')
        ?.split(',')
        .map((event) => event.trim().toLowerCase());
};

const instance = ($el: Element) => {
    (getEvents($el) || []).map((event) => {
        switch (event) {
            case 'addtocart':
                addToCart($el);
                break;
            case 'click':
                click($el);
                break;
            case 'detail':
                detail($el);
                break;
            case 'impression':
                impression($el);
                break;
            case 'purchase':
                purchase($el);
                break;
            case 'purchaseclick':
                purchaseClick($el);
                break;
            default:
                break;
        }
    });
};

const analytics = () => {
    const SELECTOR = '.js-analytics-track';

    window.dataLayer = window.dataLayer || [];

    document.querySelectorAll(SELECTOR).forEach(($el: Element) => {
        instance($el);
    });
};

export default analytics;
