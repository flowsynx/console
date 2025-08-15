window.fsDrawflow = (function () {
    let editor;
    let dotnet;

    function ensure(containerId) {
        const el = document.getElementById(containerId);
        if (!el) throw new Error("Drawflow container not found " + containerId);
        return el;
    }

    function init(containerId, dotNetRef) {
        dotnet = dotNetRef;
        const el = ensure(containerId);
        editor = new Drawflow(el);
        editor.reroute = true;
        editor.start();

        // Events -> Blazor
        editor.on('nodeSelected', function (id) {
            const data = editor.getNodeFromId(id);
            dotnet.invokeMethodAsync('OnNodeSelected', id.toString());
        });
        editor.on('nodeUnselected', function () {
            dotnet.invokeMethodAsync('OnNodeUnselected');
        });
        editor.on('connectionCreated', function (conn) {
            // conn: { output_id, input_id, output_class, input_class } => output_id -> input_id
            dotnet.invokeMethodAsync('OnConnectionCreated', conn.output_id.toString(), conn.input_id.toString());
        });
        editor.on('connectionRemoved', function (conn) {
            dotnet.invokeMethodAsync('OnConnectionRemoved', conn.output_id.toString(), conn.input_id.toString());
        });
        editor.on('nodeRemoved', function (id) {
            dotnet.invokeMethodAsync('OnNodeRemoved', id.toString());
        });

        return true;
    }

    function clear() { editor && editor.clear(); }

    function addNode(name, data, x, y) {
        const html = `<div class="title-box">${name}</div><div>${data.type || ''}</div>`;
        const inPorts = 1; // DAG dependency in
        const outPorts = 1; // DAG dependency out
        const id = editor.addNode(name, inPorts, outPorts, x, y, name, data, html);
        return id;
    }

    function updateNodeTitle(id, name, type) {
        const node = editor.getNodeFromId(+id);
        if (!node) return;
        node.name = name;
        node.html = `<div class=\"title-box\">${name}</div><div>${type || ''}</div>`;
        editor.updateNodeDataFromId(+id, node.data);
    }

    function exportDrawflow() { return JSON.stringify(editor.export()); }
    function importDrawflow(json) { editor.clear(); editor.import(JSON.parse(json)); }

    function connect(fromId, toId) { editor.addConnection(+fromId, +toId, 'output_1', 'input_1'); }
    function disconnect(fromId, toId) { editor.removeSingleConnection({ output_id: +fromId, input_id: +toId, output_class: 'output_1', input_class: 'input_1' }); }

    function getNodeData(id) { const n = editor.getNodeFromId(+id); return n ? JSON.stringify(n) : null; }
    function setNodeData(id, data) { const obj = (typeof data === 'string') ? JSON.parse(data) : data; editor.updateNodeDataFromId(+id, obj); }

    return { init, clear, addNode, connect, disconnect, exportDrawflow, importDrawflow, updateNodeTitle, getNodeData, setNodeData };
})();