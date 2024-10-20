/// <reference path="../dialogs.js" />
/// <reference path="../assistant.js" />

Ext.onReady(function(){
    var store = Ext.create('Ext.data.Store', {
        pageSize: 50,
        autoDestroy: true,
        fields: [
            {name: 'id', mapping: 'id', type: 'string'},
            {name: 'description', mapping: 'description', type: 'string'},
            {name: 'name', mapping: 'name', type: 'string'},
        ],
        proxy: {
            type: 'ajax',
            url: '/Product/List',
            extraParams: {
            },
            reader: {
                root: 'listProduct',
                totalProperty: 'totalCount'
            },
        },
    });

    var grid = Ext.create('Ext.grid.Panel', {
        viewConfig: {
            loadingText: "Загрузка данных..."
        },
        layout : 'fit',
        store: store,
        columns: [{
            header: 'Наименование',
            dataIndex: 'name',
            width: 200,
        }, {
            header: 'Описание',
            dataIndex: 'description',
            flex: 1
        }],
        title: 'Список продуктов',
        frame: true,
        tbar: [{
            text: 'Создать',
            icon: '/app/home/add.png',
            handler : function(){
                dialogs.product.addOrUpdateDialog({
                    onSave: function() {
                        store.reload();
                    }
                });
            }
        }, {
            text: 'Изменить',
            icon: '/app/home/edit.png',
            handler : function(){
                var selectedRecord = grid.getSelectionModel().getSelection()[0];
                if (!selectedRecord) {
                    dialogs.info("Выберите строку в таблице");
                } else {
                    dialogs.product.addOrUpdateDialog({
                        entity: selectedRecord.data,
                        onSave: function() {
                            store.reload();
                        }
                    });
                }
            }
        }, {
            text: 'Удалить',
            icon: '/app/home/delete.png',
            handler : function(){
                var selectedRecord = grid.getSelectionModel().getSelection()[0];
                if (!selectedRecord) {
                    dialogs.info("Выберите строку в таблице");
                } else {
                    dialogs.product.delateDialog({
                        entity: selectedRecord.data,
                        onSave: function() {
                            store.reload();
                        }
                    });
                }
            }
        }, {
            xtype: 'textfield',
            fieldLabel: 'Фильтр',
            listeners: {
                specialkey: function(f,e){
                    if(e.getKey() == e.ENTER) {
                        store.proxy.extraParams.name = f.getValue();
                        store.proxy.extraParams.page = 1;
                        store.proxy.extraParams.start = 0;
                        store.reload();
                    }
                }
            }
        }],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: store,
            displayInfo: true,
            displayMsg: 'Показано записей {0} - {1} из {2}',
            emptyMsg: "Нет задач для отображения",
        }), 
    });


    var viewport = Ext.create('Ext.container.Viewport', {
        layout : 'fit',
        items : [
            grid
        ]
    });

    store.load();
});