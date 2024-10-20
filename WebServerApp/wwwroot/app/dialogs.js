/// <reference path="assistant.js" />

var dialogs = {
    getWidth: function(width) {
        return window.width < width ? window.width :  width;
    },
    getHeight: function(height) {
        return window.height < height ? window.height :  height;
    },
    genId: function () {
        return Ext.id();
    },

    error: function(text) {
        Ext.MessageBox.show({
            title: window.title,
            msg: text,
            icon: Ext.MessageBox.ERROR,
            buttons: Ext.Msg.OK
        });
    },

    info: function(text) {
        Ext.MessageBox.show({
            title: window.title,
            msg: text,
            icon: Ext.MessageBox.INFO,
            buttons: Ext.Msg.OK
        });
    },

    confirm: function(text, onOk) {
        Ext.MessageBox.show({
            title: window.title,
            msg: text,
            buttons: Ext.MessageBox.OKCANCEL,
            icon: Ext.MessageBox.QUESTION,
            fn: function(btn){
                if(btn == 'ok'){
                    if (onOk) {
                        onOk();
                    }
                } else {
                }
            }
        });
    },

    productVersion: {
        addOrUpdateDialog: function(data) {
            const onSave = data && data.onSave;
            const entity = data && data.entity;

            const windowName = dialogs.genId();
            const formName = dialogs.genId();
            const nameName = dialogs.genId();
            const wdithName = dialogs.genId();
            const heightName = dialogs.genId();
            const lengthName = dialogs.genId();
            const descriptionName = dialogs.genId();

            Ext.create('Ext.window.Window', {
                title: 'Версия продукта',
                id: windowName,
                width: dialogs.getWidth(350),
                height: dialogs.getHeight(300),
                constrainTo: Ext.getBody(),
                layout:'fit',
                resizable: true,
                modal: true,
                items: [
                    Ext.create('Ext.form.FormPanel', {
                        id: formName,
                        border: false,
                        bodyPadding: 3,
                        layout: {
                            type: 'vbox',
                            pack: 'start',
                            align: 'stretch',
                        },
                        items: [{
                            xtype: 'textfield',
                            name: 'name',
                            fieldLabel: 'Наименование',
                            allowBlank: false,
                            anchor: '100%',
                            id: nameName,
                            value: entity && entity.name
                        }, {
                            xtype: 'numberfield',
                            anchor: '100%',
                            name: 'wdith',
                            fieldLabel: 'Ширина',
                            allowBlank: false,
                            value: entity && entity.wdith,
                            id: wdithName,
                            minValue: 0
                        }, {
                            xtype: 'numberfield',
                            anchor: '100%',
                            name: 'height',
                            fieldLabel: 'Высота',
                            allowBlank: false,
                            value: entity && entity.height,
                            id: heightName,
                            minValue: 0
                        }, {
                            xtype: 'numberfield',
                            anchor: '100%',
                            name: 'length',
                            fieldLabel: 'Длина',
                            allowBlank: false,
                            value: entity && entity.length,
                            id: lengthName,
                            minValue: 0
                        }, {
                            xtype: 'textareafield',
                            fieldLabel: 'Описание',
                            name: 'description',
                            anchor: '100%',
                            flex: 1,
                            id: descriptionName,
                            value: entity && entity.description
                        }]
                    })
                ],
                buttons: [{
                    text: 'Сохранить',
                    handler: function() {
                        if (!Ext.getCmp(formName).isValid()) {
                            return;
                        }
                        const instance = {...(entity || {})};
                        instance.name = Ext.getCmp(nameName).getValue();
                        instance.wdith = Ext.getCmp(wdithName).getValue();
                        instance.height = Ext.getCmp(heightName).getValue();
                        instance.length = Ext.getCmp(lengthName).getValue();
                        instance.description = Ext.getCmp(descriptionName).getValue();
                        
                        if (onSave) {
                            onSave({
                                entity: instance
                            });
                        }

                        Ext.getCmp(windowName).close();
                    }
                }, {
                    text: 'Отмена',
                    handler: function() {                           
                        Ext.getCmp(windowName).close();
                    }
                }]
            }).show();
        }
    },

    product: {
        delateDialog: function(data) {
            const onSave = data && data.onSave;
            const entity = data && data.entity;
            if (!entity){
                return;
            }
            dialogs.confirm('Удалить продукт \"' + (entity.name || '') + '\" ?', function() {
                assistant.mask();
                assistant.postRequest({
                    data: {
                        id: entity.id
                    },
                    url: 'Product/Delete',
                    success: function() {
                        assistant.unmask();
                        if (onSave) {
                            onSave();
                        }
                    },
                    error: function(text){
                        assistant.unmask();
                        dialogs.error(text);
                    }
                });
            });
        },

        addOrUpdateDialog: function(data) {
            const onSave = data && data.onSave;
            const entity = data && data.entity;

            const windowName = dialogs.genId();
            const formName = dialogs.genId();
            const nameName = dialogs.genId();
            const descriptionName = dialogs.genId();

            var versionGrid = assistant.createMemoryGrid({
                title: 'Версии',
                flex: 2,
                key: 'id',
                onCreate: function() {
                    dialogs.productVersion.addOrUpdateDialog({
                        onSave: function(data) {
                            versionGrid.getStore().add(data.entity);
                        }
                    });
                },
                onEdit: function() {
                    var selectedRecord = versionGrid.getSelectionModel().getSelection()[0];
                    if (selectedRecord) {
                        dialogs.productVersion.addOrUpdateDialog({
                            entity: selectedRecord.data,
                            onSave: function(data) {
                                selectedRecord.set(data.entity);
                            }
                        });
                    }
                },
                onDelete: function() {
                    var selectedRecord = versionGrid.getSelectionModel().getSelection()[0];
                    if (selectedRecord) {
                        versionGrid.getStore().remove(selectedRecord);
                    }
                },
                columns: [{
                    name: 'name',
                    header: 'Наименование',
                    width: 100,
                }, {
                    name: 'description',
                    header: 'Описание',
                    flex: 1,
                }]
            });

            Ext.create('Ext.window.Window', {
                title: !entity ? 'Создание продукта' : 'Редактирование продукта',
                id: windowName,
                width: dialogs.getWidth(400),
                height: dialogs.getHeight(400),
                constrainTo: Ext.getBody(),
                layout:'fit',
                resizable: true,
                modal: true,
                items: [Ext.create('Ext.form.FormPanel', {
                    id: formName,
                    border: false,
                    bodyPadding: 3,
                    layout: {
                        type: 'vbox',
                        pack: 'start',
                        align: 'stretch',
                    }, 
                    items: [{
                        xtype: 'textfield',
                        name: 'name',
                        fieldLabel: 'Наименование',
                        allowBlank: false,
                        anchor: '100%',
                        id: nameName,
                        value: entity && entity.name
                    }, {
                        xtype: 'textareafield',
                        fieldLabel: 'Описание',
                        name: 'description',
                        anchor: '100%',
                        flex: 1,
                        id: descriptionName,
                        value: entity && entity.description
                    }, versionGrid]
                })],
                buttons: [{
                    text: 'Сохранить',
                    handler: function() {
                        if (!Ext.getCmp(formName).isValid()) {
                            return;
                        }
                        Ext.getCmp(windowName).getEl().mask('Loading...');

                        const instance = {...(entity || {})};
                        instance.name = Ext.getCmp(nameName).getValue();
                        instance.description = Ext.getCmp(descriptionName).getValue();
                        const versions = versionGrid.getStore().getData().items.map(x => x.getData());
                        instance.versions = versions;

                        const url = !entity ? 'Product/Add' : 'Product/Update';
                        assistant.postRequest({
                            data: instance,
                            url: url,
                            success: function() {
                                Ext.getCmp(windowName).getEl().unmask();
                                Ext.getCmp(windowName).close();
                                if (onSave) {
                                    onSave();
                                }
                            },
                            error: function(text){
                                Ext.getCmp(windowName).getEl().unmask();
                                dialogs.error(text);
                            }
                        })
                    }
                }, {
                    text: 'Отмена',
                    handler: function() {                           
                        Ext.getCmp(windowName).close();
                    }
                }],
            }).show();
            

            if (entity) {
                assistant.mask(versionGrid.getId());
                assistant.getRequest({
                    url: 'Product/Versions?ProductId=' + entity.id,
                    success: function(data) {
                        assistant.unmask(versionGrid.getId());
                        for(let i = 0 ; i < data.length ; i++) {
                            versionGrid.getStore().add(data[i]);
                        }
                    },
                    error: function() {
                        assistant.unmask(versionGrid.getId());
                    }
                })
            }
        }
    }
};