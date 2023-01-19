export default ($el: Element) => {
    const $listContainer = $el.closest('.js-analytics-list') as HTMLElement;

    if ($listContainer && $listContainer.getAttribute('data-list')) {
        return `${$listContainer.getAttribute('data-list')}, ${document.title}`;
    }

    return document.title;
};
