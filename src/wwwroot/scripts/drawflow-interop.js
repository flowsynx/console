window.fsDrawflow = (function () {
    let editor = null;
    let dotnet = null;
    let zoomLevel = 1;
    let isReadOnly = false;

    // -----------------------------
    // Helper Functions
    // -----------------------------
    const ensureContainer = (containerId) => {
        const el = document.getElementById(containerId);
        if (!el) throw new Error(`Drawflow container not found: ${containerId}`);
        return el;
    };

    const bindNodeEvents = (id) => {
        if (isReadOnly) return;

        const el = document.querySelector(`#node-${id}`);
        if (!el) return;

        el.addEventListener("dblclick", () =>
            dotnet.invokeMethodAsync("OnNodeDoubleClicked", id.toString())
        );
    };

    const safeInvoke = (method, ...args) => {
        if (dotnet) dotnet.invokeMethodAsync(method, ...args);
    };

    // -----------------------------
    // Arrowhead Helpers
    // -----------------------------
    const addArrowheadToPath = (pathEl) => {
        if (!pathEl) return;

        const arrow = document.createElement("div");
        arrow.className = "drawflow-arrowhead";
        pathEl.arrowEl = arrow; // keep reference
        editor.precanvas.appendChild(arrow);

        const updatePosition = () => {
            try {
                const length = pathEl.getTotalLength();
                const point = pathEl.getPointAtLength(length - 1);
                const pointPrev = pathEl.getPointAtLength(length - 5);
                const angle = Math.atan2(point.y - pointPrev.y, point.x - pointPrev.x) * 180 / Math.PI;

                arrow.style.left = `${point.x}px`;
                arrow.style.top = `${point.y}px`;
                arrow.style.transform = `translate(-50%, -50%) rotate(${angle}deg)`;
            } catch { /* ignore */ }
        };

        updatePosition();
        const obs = new MutationObserver(updatePosition);
        obs.observe(pathEl, { attributes: true, attributeFilter: ["d"] });
    };

    // -----------------------------
    // Core API
    // -----------------------------
    const init = (containerId, dotNetRef, options = {}) => {
        dotnet = dotNetRef;
        isReadOnly = options.readOnly || false;

        const container = ensureContainer(containerId);

        editor = new Drawflow(container);
        editor.reroute = true;
        editor.start();
        zoomLevel = 1;
        if (!isReadOnly)
            container.classList.add("drawflow-canvas");

        // Apply read-only
        editor.editor_mode = isReadOnly ? "view" : "edit";

        // Mousewheel zoom
        container.addEventListener("wheel", (e) => {
            if (e.ctrlKey) {
                e.preventDefault();
                if (e.deltaY < 0) zoomIn();
                else zoomOut();
            }
        });

        if (!isReadOnly) {
            editor.on("nodeSelected", (id) => safeInvoke("OnNodeSelected", id.toString()));
            editor.on("nodeUnselected", () => safeInvoke("OnNodeUnselected"));
            editor.on("nodeRemoved", (id) => safeInvoke("OnNodeRemoved", id.toString()));
            editor.on("nodeMoved", (id) => {
                const node = editor.getNodeFromId(id);
                if (node) {
                    safeInvoke("OnNodeMoved", id.toString(), node.pos_x, node.pos_y);
                }
            });
            editor.on("connectionCreated", ({ output_id, input_id }) => {
                safeInvoke("OnConnectionCreated", output_id.toString(), input_id.toString());
                setTimeout(() => {
                    const pathEl = container.querySelector(
                        `.connection.node_in_${input_id}.node_out_${output_id} path.main-path`
                    );
                    if (pathEl) addArrowheadToPath(pathEl);
                }, 50);
            });
            editor.on("connectionRemoved", ({ output_id, input_id }) => {
                safeInvoke("OnConnectionRemoved", output_id.toString(), input_id.toString());
                const pathEl = container.querySelector(
                    `.connection.node_in_${input_id}.node_out_${output_id} path.main-path`
                );
                if (pathEl && pathEl.arrowEl) pathEl.arrowEl.remove();
            });
        }

        return true;
    };

    const clear = () => editor?.clear();

    const addNode = (name, data, x, y) => {
        const id = editor.addNode(
            name, 1, 1, x, y, name, data,
            `<div class="title-box">${name}</div><div>${data.type || ""}</div>`
        );
        bindNodeEvents(id);
        return id;
    };

    const updateNodeTitle = (id, name, type) => {
        const node = editor.getNodeFromId(+id);
        if (!node) return;
        node.name = name;
        node.html = `<div class="title-box">${name}</div><div>${type || ""}</div>`;
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

    const zoomIn = () => { zoomLevel = Math.min(zoomLevel + 0.1, 2); applyZoom(); };
    const zoomOut = () => { zoomLevel = Math.max(zoomLevel - 0.1, 0.2); applyZoom(); };
    const zoomReset = () => { zoomLevel = 1; editor.canvas_x = 0; editor.canvas_y = 0; applyZoom(); };
    const center = () => { zoomLevel = 1; editor.canvas_x = 0; editor.canvas_y = 0; applyZoom(); };

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
        zoomIn,
        zoomOut,
        zoomReset,
        center,
    };
})();