window.initializeJsonEditor = function (element, initialJson, dotNetRef) {
    const options = {
        mode: 'code',
        modes: ['code', 'view'],
        enableSort: false,
        enableTransform: false,
        enableFilter: false,
        onChange: function () {
            try {
                const json = editor.get();
                dotNetRef.invokeMethodAsync('OnJsonChange', JSON.stringify(json));
            } catch (err) {
                console.error('Error parsing JSON', err);
            }
        }
    };

    const editor = new JSONEditor(element, options);
    if (initialJson) {
        try {
            editor.set(JSON.parse(initialJson));
        } catch (err) {
            console.error('Error setting initial JSON', err);
            editor.setText(initialJson);
        }
    }

    return editor;
};