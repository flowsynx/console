window.fsDrawflow = (function () {
    let editor = null;
    let dotnet = null;
    let contextMenuEl = null;
    let currentNodeId = null;
    let zoomLevel = 1;

    // -----------------------------
    // Helper Functions
    // -----------------------------
    const ensureContainer = (containerId) => {
        const el = document.getElementById(containerId);
        if (!el) throw new Error(`Drawflow container not found: ${containerId}`);
        return el;
    };

    const createContextMenu = () => {
        if (contextMenuEl) return;

        contextMenuEl = document.createElement("div");
        contextMenuEl.id = "node-context-menu";

        const propItem = document.createElement("div");
        propItem.innerText = "Properties";
        propItem.className = "menu-item";
        propItem.onclick = () => {
            hideContextMenu();
            if (currentNodeId) {
                dotnet.invokeMethodAsync("OnNodeProperties", currentNodeId.toString());
            }
        };

        const delItem = document.createElement("div");
        delItem.innerText = "Delete";
        delItem.className = "menu-item";
        delItem.onclick = () => {
            hideContextMenu();
            if (currentNodeId) {
                editor.removeNodeId(currentNodeId); // Drawflow’s method
                dotnet.invokeMethodAsync("OnNodeRemoved", currentNodeId.toString());
            }
        };

        contextMenuEl.appendChild(propItem);
        contextMenuEl.appendChild(delItem);
        document.body.appendChild(contextMenuEl);

        // Hide when clicking outside
        document.addEventListener("click", (e) => {
            if (!contextMenuEl.contains(e.target)) {
                hideContextMenu();
            }
        });
    };

    const showContextMenu = (x, y, nodeId) => {
        createContextMenu();
        currentNodeId = nodeId;
        contextMenuEl.style.left = `${x}px`;
        contextMenuEl.style.top = `${y}px`;
        contextMenuEl.style.display = "block";
    };

    const hideContextMenu = () => {
        if (contextMenuEl) {
            contextMenuEl.style.display = "none";
            currentNodeId = null;
        }
    };

    const bindNodeEvents = (id) => {
        const el = document.querySelector(`#node-${id}`);
        if (!el) return;

        el.addEventListener("dblclick", () =>
            dotnet.invokeMethodAsync("OnNodeDoubleClicked", id.toString())
        );

        el.addEventListener("contextmenu", (e) => {
            e.preventDefault();
            showContextMenu(e.pageX, e.pageY, id);
        });
    };

    const safeInvoke = (method, ...args) => {
        if (dotnet) dotnet.invokeMethodAsync(method, ...args);
    };

    // -----------------------------
    // Core API
    // -----------------------------
    const init = (containerId, dotNetRef) => {
        dotnet = dotNetRef;
        const container = ensureContainer(containerId);

        editor = new Drawflow(container);
        editor.reroute = true;
        editor.start();
        zoomLevel = 1;

        // Apply grid background
        container.classList.add("drawflow-canvas");

        // Mousewheel zoom
        container.addEventListener("wheel", (e) => {
            if (e.ctrlKey) {
                e.preventDefault();
                if (e.deltaY < 0) {
                    zoomIn();
                } else {
                    zoomOut();
                }
            }
        });

        // Drawflow event bindings
        editor.on("nodeSelected", (id) => safeInvoke("OnNodeSelected", id.toString()));
        editor.on("nodeUnselected", () => safeInvoke("OnNodeUnselected"));
        editor.on("connectionCreated", ({ output_id, input_id }) =>
            safeInvoke("OnConnectionCreated", output_id.toString(), input_id.toString())
        );
        editor.on("connectionRemoved", ({ output_id, input_id }) =>
            safeInvoke("OnConnectionRemoved", output_id.toString(), input_id.toString())
        );
        editor.on("nodeRemoved", (id) => safeInvoke("OnNodeRemoved", id.toString()));

        return true;
    };

    const clear = () => editor?.clear();

    const addNode = (name, data, x, y) => {
        const inPorts = 1;
        const outPorts = 1;
        const htmlContent = `<div class="title-box">${name}</div><div>${data.type || ""}</div>`;

        const id = editor.addNode(name, inPorts, outPorts, x, y, name, data, htmlContent);
        bindNodeEvents(id);
        return id;
    };

    const updateNodeTitle = (id, name, type) => {
        const node = editor.getNodeFromId(+id);
        if (!node) return;

        node.name = name;
        node.html = `
            <div class="title-box">${name}</div>
            <div>${type || ""}</div>
        `;
        editor.updateNodeDataFromId(+id, node.data);

        bindNodeEvents(id);
    };

    const exportDrawflow = () => JSON.stringify(editor.export());
    const importDrawflow = (json) => {
        editor.clear();
        editor.import(JSON.parse(json));
    };

    const connect = (fromId, toId) =>
        editor.addConnection(+fromId, +toId, "output_1", "input_1");

    const disconnect = (fromId, toId) =>
        editor.removeSingleConnection({
            output_id: +fromId,
            input_id: +toId,
            output_class: "output_1",
            input_class: "input_1",
        });

    const getNodeData = (id) => {
        const node = editor.getNodeFromId(+id);
        return node ? JSON.stringify(node) : null;
    };

    const setNodeData = (id, data) => {
        const obj = typeof data === "string" ? JSON.parse(data) : data;
        editor.updateNodeDataFromId(+id, obj);
    };

    // -----------------------------
    // Zoom & Pan
    // -----------------------------
    const applyZoom = () => {
        if (!editor) return;
        editor.precanvas.style.transform = `translate(${editor.canvas_x}px, ${editor.canvas_y}px) scale(${zoomLevel})`;
    };

    const zoomIn = () => {
        zoomLevel = Math.min(zoomLevel + 0.1, 2);
        applyZoom();
    };

    const zoomOut = () => {
        zoomLevel = Math.max(zoomLevel - 0.1, 0.2);
        applyZoom();
    };

    const zoomReset = () => {
        zoomLevel = 1;
        editor.canvas_x = 0;
        editor.canvas_y = 0;
        applyZoom();
    };

    const center = () => {
        zoomLevel = 1;
        editor.canvas_x = 0;
        editor.canvas_y = 0;
        applyZoom();
    };

    // -----------------------------
    // Public API
    // -----------------------------
    return {
        init,
        clear,
        addNode,
        updateNodeTitle,
        connect,
        disconnect,
        exportDrawflow,
        importDrawflow,
        getNodeData,
        setNodeData,

        // Zoom / pan
        zoomIn,
        zoomOut,
        zoomReset,
        center,
    };
})();