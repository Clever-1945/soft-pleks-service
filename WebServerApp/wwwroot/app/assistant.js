var assistant = {
    mask: function(id) {
        const element = Ext.getCmp(id) || Ext.getBody();
        if (element && element.getEl && element.getEl().mask) {
            element.getEl().mask('Loading...');
        } else if (element.mask) {
            element.mask('Loading...');
        }
    },

    unmask: function(id) {
        const element = Ext.getCmp(id) || Ext.getBody();
        if (element && element.getEl && element.getEl().mask) {
            element.getEl().unmask();
        } else if (element.mask) {
            element.unmask();
        }
    },

    postRequest: function(data) {
        const url = data && data.url;
        const success = data && data.success;
        const error = data && data.error;
        Ext.Ajax.request({
            method: 'POST',
            url: url,
            params: {
                requestParam: 'notInRequestBody'
            },
            jsonData: ((data && data.data) || {}),
            success: function(r) {
                let successData = null;
                if (r && r.responseText) {
                    try {
                        successData = JSON.parse(r.responseText);
                    } catch (e) {
                        if (error) {
                            error(t.toString());
                        }
                    }
                }
                if (success) {
                    success(successData);
                }
            },
            failure: function(r) {
                const responseText = 
                    (r && r.responseText) ||
                    (r && r.statusText) ||
                    '';
                if (error) {
                    error(responseText);
                }
            }
        });
    },

    getRequest: function(data) {
        const url = data && data.url;
        const success = data && data.success;
        const error = data && data.error;
        Ext.Ajax.request({
            method: 'GET',
            url: url,
            success: function(r) {
                let successData = null;
                if (r && r.responseText) {
                    try {
                        successData = JSON.parse(r.responseText);
                    } catch (e) {
                        if (error) {
                            error(t.toString());
                        }
                    }
                }
                if (success) {
                    success(successData);
                }
            },
            failure: function(r) {
                const responseText = 
                    (r && r.responseText) ||
                    (r && r.statusText) ||
                    '';
                if (error) {
                    error(responseText);
                }
            }
        });
    },

    /**
     * title
     * flex
     * height
     * {
     *  columns: [{
     *      name
     *      header
     *      width
     *      flex
     *  }]
     *  onCreate
     *  onEdit
     *  onDelete
     * }
     */
    createMemoryGrid: function (shema) {
        const shemaColumns = shema && shema.columns;
        const fields = [];
        const columns = [];

        if (shema && shema.key) {
            fields.push({
                name: shema.key,
                mapping: shema.key,
                type: 'string',
            });
        }

        if (shemaColumns && shemaColumns.length) {
            for(let i = 0 ; i < shemaColumns.length ; i ++) {
                const shemaColumn = shemaColumns[i];
                const field = {};
                const column = {};
                if (shemaColumn && shemaColumn.name) {
                    field.name = shemaColumn.name;
                    field.mapping = shemaColumn.name;
                    column.dataIndex = shemaColumn.name;
                }
                if (shemaColumn && shemaColumn.header) {
                    column.header = shemaColumn.header;
                }
                if (shemaColumn && shemaColumn.width) {
                    column.width = shemaColumn.width;
                }
                if (shemaColumn && shemaColumn.flex) {
                    column.flex = shemaColumn.flex;
                }
                fields.push(field);
                columns.push(column);
            }
        }

        const store = Ext.create('Ext.data.Store', {
            fields: fields,
            autoDestroy: true,
            proxy: {
                type: 'memory',
                reader: {
                    root: 'list',
                },
            }
        });

        var grid = Ext.create('Ext.grid.Panel', {
            layout : 'fit',
            flex: shema && shema.flex,
            height: shema && shema.height,
            store: store,
            columns: columns,
            title: shema && shema.title,
            frame: true,
            tbar: [{
                text: 'Создать',
                icon: '/app/home/add.png',
                handler : function() {
                    if (shema && shema.onCreate) {
                        shema.onCreate(store, grid);
                    }
                }
            }, {
                text: 'Изменить',
                icon: '/app/home/edit.png',
                handler : function() {
                    if (shema && shema.onEdit) {
                        shema.onEdit(store, grid);
                    }
                }
            }, {
                text: 'Удалить',
                icon: '/app/home/delete.png',
                handler : function() {
                    if (shema && shema.onDelete) {
                        shema.onDelete(store, grid);
                    }
                }
            }],
        });

        return grid;
    }
};