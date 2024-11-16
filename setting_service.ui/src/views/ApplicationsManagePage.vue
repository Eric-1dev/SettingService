<template>
    <div>
        <n-space :justify="'space-between'">
            <n-space>
                <n-input
                    placeholder="Поиск"
                    @input="filterByName"
                    :style="{ width: '300px' }"
                >
                    <template #suffix>
                        <n-icon :component="Search12Regular" />
                    </template>
                </n-input>
                <n-button
                    :disabled="!isFetchSuccess"
                    @click="showAddDialog"
                    type="primary"
                    ghost
                    >Добавить новое приложение</n-button
                >
            </n-space>
            <n-space>
                <n-button
                    :disabled="selectedAppRowData == null"
                    type="info"
                    ghost
                    @click="() => editAppSettings()"
                    >Настройки приложения</n-button
                >
                <n-button
                    :disabled="selectedAppRowData == null"
                    @click="showEditDialog"
                    >Редактировать</n-button
                >
                <n-button
                    :type="'error'"
                    ghost
                    :disabled="selectedAppRowData == null"
                    @click="showDeleteDialog = true"
                    >Удалить</n-button
                >
            </n-space>
        </n-space>

        <n-divider />

        <n-space v-if="isFetchingApps" :justify="'center'">
            <n-spin size="large" />
        </n-space>
        <div v-else-if="tableData?.length === 0">
            Ни одно приложения пока не создано
        </div>
        <n-data-table
            v-else
            :columns="columns"
            :data="tableData"
            ref="table"
            :bordered="false"
            :row-props="rowProps"
            v-model:checked-row-keys="checkedRowKeys"
        ></n-data-table>

        <n-modal
            v-model:show="showAddEditDialog"
            preset="dialog"
            :title="addEditDialogParameters.title"
            :positive-text="addEditDialogParameters.positiveText"
            negative-text="Отмена"
            @positive-click="addEditDialogParameters.positiveClick"
            @negative-click="showAddEditDialog = false"
            @close="showAddEditDialog = false"
            @keydown.enter.prevent="addEditDialogParameters.positiveClick"
            :mask-closable="false"
        >
            <n-form ref="formRef" :model="newAppModel" :rules="newAppRules">
                <n-form-item path="name" label="Название">
                    <n-input
                        v-model:value.trim="newAppModel.name"
                        @keydown.enter.prevent
                        :placeholder="'Введите название'"
                    />
                </n-form-item>
                <n-form-item path="description" label="Описание (опционально)">
                    <n-input
                        v-model:value.trim="newAppModel.description"
                        @keydown.enter.prevent
                        :placeholder="'Введите описание'"
                    />
                </n-form-item>
            </n-form>
        </n-modal>

        <n-modal
            v-model:show="showDeleteDialog"
            preset="confirm"
            title="Удаление приложения"
            positive-text="Удалить"
            negative-text="Отмена"
            @close="showDeleteDialog = false"
            @negative-click="showDeleteDialog = false"
            @positive-click="deleteApplication"
            @keydown.enter.prevent="addEditDialogParameters.positiveClick"
            :mask-closable="false"
        >
            Вы действительно хотите удалить приложение
            {{ selectedAppRowData?.name }}? Изменения необратимы
        </n-modal>
    </div>
</template>

<script lang="ts" setup>
import { ApplicationFrontModel } from "@/types/ApplicationFrontModel";
import {
    DataTableColumns,
    FormInst,
    FormRules,
    NButton,
    NDataTable,
    NForm,
    NFormItem,
    NInput,
    NModal,
    useMessage,
    NIcon,
    NSpace,
    NDivider,
    NSpin,
    useNotification,
} from "naive-ui";
import { computed, HTMLAttributes, ref, VNodeRef } from "vue";
import { Search12Regular } from "@vicons/fluent";
import { OnUpdateValue } from "naive-ui/es/input/src/interface";
import { RowKey } from "naive-ui/es/data-table/src/interface";
import {
    OperationResult,
    OperationResultGeneric,
} from "@/types/OperationResult";
import { createConfiguredFetch } from "@/utils/createConfiguredFetch";
import { notify } from "@/utils/notify";
import router from "@/router";
import { AddEditDialogParameters } from "@/types/ShowAddEditDialogParameters";

interface CreateEditAppModel extends Omit<ApplicationFrontModel, "key"> {
    key?: number | null;
}

const message = useMessage();
const notification = useNotification();

const appFetch = createConfiguredFetch(
    message,
    process.env.VUE_APP_API_APPLICATIONS_URL
);

let isFetchingApps = ref<boolean>(true);

const loadApps = () => {
    const { isFetching, data, onFetchResponse, onFetchError } = appFetch(
        "GetAll"
    )
        .get()
        .json<OperationResultGeneric<ApplicationFrontModel[]>>();

    isFetchingApps = isFetching;

    onFetchResponse(() => {
        tableData.value = data.value?.entity ?? [];
        isFetchSuccess.value = true;
    });

    onFetchError(() => {
        isFetchSuccess.value = false;
    });
};

loadApps();

const table = ref<VNodeRef | null>(null);
const tableData = ref<ApplicationFrontModel[]>([]);
const checkedRowKeys = ref<RowKey[]>([]);
const rowProps = (row: ApplicationFrontModel): HTMLAttributes => {
    return {
        style: "cursor: pointer;",
        onClick: () => {
            checkedRowKeys.value = [row.key];
        },
        onDblclick: () => {
            editAppSettings(row.name);
        },
    };
};

const formRef = ref<FormInst | null>(null);
const newAppRules: FormRules = {
    name: [
        {
            required: true,
            validator(_, value: string) {
                if (!value) {
                    return new Error("Необходимо указать название");
                } else if (value.length < 2) {
                    return new Error(
                        "Название приложения должно состоять не меньше чем из 2-х символов"
                    );
                }
                return true;
            },
            trigger: ["input", "blur"],
        },
    ],
};

const showAddEditDialog = ref<boolean>(false);
const addEditDialogParameters = ref<AddEditDialogParameters>({
    positiveText: "",
    title: "",
    positiveClick: () => {
        return;
    },
});

const showDeleteDialog = ref<boolean>(false);

const isFetchSuccess = ref<boolean>(false);

const newAppModel = ref<CreateEditAppModel>({
    name: "",
    description: "",
});

const selectedAppRowData = computed((): ApplicationFrontModel | null => {
    const key = checkedRowKeys.value?.[0];
    return tableData.value?.find((app) => app.key == key) ?? null;
});

const showAddDialog = () => {
    newAppModel.value.name = "";
    newAppModel.value.description = "";
    addEditDialogParameters.value.title = "Добавление нового приложения";
    addEditDialogParameters.value.positiveText = "Добавить";
    addEditDialogParameters.value.positiveClick = () =>
        addApplication(newAppModel.value);
    showAddEditDialog.value = true;
};

const showEditDialog = () => {
    newAppModel.value.key = selectedAppRowData.value?.key;
    newAppModel.value.name = selectedAppRowData.value?.name ?? "";
    newAppModel.value.description = selectedAppRowData.value?.description ?? "";
    addEditDialogParameters.value.title = "Редактирование приложения";
    addEditDialogParameters.value.positiveText = "Сохранить";
    addEditDialogParameters.value.positiveClick = () =>
        editApplication(newAppModel.value);
    showAddEditDialog.value = true;
};

const editAppSettings = (name?: string | null): void => {
    router.push({
        name: "settings-page",
        query: { appName: name ?? selectedAppRowData.value?.name },
    });
};

const addApplication = (app: CreateEditAppModel): void => {
    formRef.value
        ?.validate()
        .then(() => {
            showAddEditDialog.value = false;

            const { data, onFetchResponse } = appFetch("Add")
                .post(app)
                .json<OperationResultGeneric<ApplicationFrontModel>>();

            onFetchResponse(() => {
                if (data.value?.entity) {
                    tableData.value.push(data.value.entity);
                }
                notify(notification, "success", "Добавлено новое приложение");
            });
        })
        .catch(() => {
            notify(
                notification,
                "error",
                "Проверьте правильность заполнения полей"
            );
        });
};

const editApplication = (app: CreateEditAppModel): void => {
    formRef.value
        ?.validate()
        .then(() => {
            showAddEditDialog.value = false;

            const { data, onFetchResponse } = appFetch("Edit")
                .put(app)
                .json<OperationResultGeneric<ApplicationFrontModel>>();

            onFetchResponse(() => {
                if (data.value?.entity) {
                    const index = tableData.value.findIndex(
                        (x) => x.key === data.value?.entity?.key
                    );

                    tableData.value[index] = data.value.entity;
                }
                notify(notification, "success", "Изменения сохранены");
            });
        })
        .catch(() => {
            notify(
                notification,
                "error",
                "Проверьте правильность заполнения полей"
            );
        });
};

const deleteApplication = () => {
    showDeleteDialog.value = false;

    const { onFetchResponse } = appFetch(
        `Delete/${selectedAppRowData.value?.key}`
    )
        .delete()
        .json<OperationResult>();

    onFetchResponse(() => {
        tableData.value =
            tableData.value?.filter(
                (x) => x.key != selectedAppRowData.value?.key
            ) ?? [];
        notify(notification, "success", "Приложение удалено");
    });
};

const columns: DataTableColumns<ApplicationFrontModel> = [
    {
        type: "selection",
        multiple: false,
    },
    {
        title: "ID",
        key: "key",
    },
    {
        title: "Название",
        key: "name",
        filter(value, row) {
            return (
                row.name
                    .toLowerCase()
                    .indexOf((value as string).toLowerCase()) != -1
            );
        },
    },
    {
        title: "Описание",
        key: "description",
    },
];

const filterByName: OnUpdateValue = (value) => {
    table.value.filter({
        name: value,
    });
};
</script>

<style scoped></style>
