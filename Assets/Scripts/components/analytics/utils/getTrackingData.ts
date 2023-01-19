interface ITrackingData {
    item_brand: string | null;
    item_category: string | null;
    item_id: string | null;
    item_list_name: string | null;
    item_name: string | null;
    price: number;
    quantity: number
}

export default ($el: Element): ITrackingData => {
    return {
        item_brand: $el.getAttribute('data-brand'),
        item_category: $el.getAttribute('data-category'),
        item_id: $el.getAttribute('data-item-id'),
        item_list_name: null,
        item_name: $el.getAttribute('data-name'),
        price: 0,
        quantity: 1,
    };
};
