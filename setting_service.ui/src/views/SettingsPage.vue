<template>
    <div>
        <n-space :justify="'center'" :align="'center'">
            <n-h2>{{ appName }}</n-h2>
        </n-space>

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
                    >Добавить настройку</n-button
                >
            </n-space>
            <n-space>
                <n-button
                    :disabled="selectedSettingRowData == null"
                    @click="() => showEditDialog(selectedSettingRowData)"
                    >Редактировать</n-button
                >
                <n-button
                    :type="'error'"
                    ghost
                    :disabled="selectedSettingRowData == null"
                    @click="showDeleteDialog = true"
                    >Удалить</n-button
                >
            </n-space>
        </n-space>

        <n-divider />

        <n-space v-if="isFetchingSettings" :justify="'center'">
            <n-spin size="large" />
        </n-space>
        <div v-else-if="tableData.length === 0">
            Ни одной настройки для приложения {{ appName }} пока не создано
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
            :show="showAddEditDialog"
            preset="dialog"
            :title="addEditDialogParameters.title"
            :positive-text="addEditDialogParameters.positiveText"
            negative-text="Отмена"
            @positive-click="addEditDialogParameters.positiveClick"
            @negative-click="showAddEditDialog = false"
            @close="showAddEditDialog = false"
        >
            <n-form
                ref="formRef"
                :model="newSettingModel"
                :rules="newSettingRules"
            >
                <n-form-item path="name" label="Название">
                    <n-input
                        v-model:value="newSettingModel.name"
                        @keydown.enter.prevent
                        :placeholder="'Введите название'"
                    />
                </n-form-item>
                <n-form-item path="description" label="Описание">
                    <n-input
                        v-model:value="newSettingModel.description"
                        @keydown.enter.prevent
                        :placeholder="'Введите описание'"
                    />
                </n-form-item>
                <n-form-item path="valueType" label="Тип значение настройки">
                    <n-select
                        v-model:value="newSettingModel.valueType"
                        :filterable="true"
                        @keydown.enter.prevent
                        :options="settingValueTypes"
                        :placeholder="'Выберите тип значения настройки'"
                    />
                </n-form-item>
                <n-form-item
                    path="isFromExternalSource"
                    label="Значение будет получаться из внешнего источника"
                >
                    <n-checkbox
                        v-model:checked="newSettingModel.isFromExternalSource"
                        @keydown.enter.prevent
                    />
                </n-form-item>
                <n-form-item
                    v-if="!newSettingModel.isFromExternalSource"
                    path="value"
                    label="Значение"
                >
                    <n-input
                        v-if="
                            newSettingModel.valueType ===
                            SettingValueTypeEnum[SettingValueTypeEnum.String]
                        "
                        v-model:value="valueModel.stringValue"
                        @keydown.enter.prevent
                        :placeholder="'Введите строковое значение'"
                    />
                    <n-input-number
                        v-else-if="
                            newSettingModel.valueType ===
                                SettingValueTypeEnum[
                                    SettingValueTypeEnum.Decimal
                                ] ||
                            newSettingModel.valueType ===
                                SettingValueTypeEnum[
                                    SettingValueTypeEnum.Double
                                ] ||
                            newSettingModel.valueType ===
                                SettingValueTypeEnum[
                                    SettingValueTypeEnum.Int
                                ] ||
                            newSettingModel.valueType ===
                                SettingValueTypeEnum[SettingValueTypeEnum.Long]
                        "
                        v-model:value="valueModel.numberValue"
                        @keydown.enter.prevent
                        :precision="
                            newSettingModel.valueType ===
                                SettingValueTypeEnum[
                                    SettingValueTypeEnum.Int
                                ] ||
                            newSettingModel.valueType ===
                                SettingValueTypeEnum[SettingValueTypeEnum.Long]
                                ? 0
                                : undefined
                        "
                        :placeholder="'Введите числовое значение'"
                    />
                    <n-switch
                        v-else-if="
                            newSettingModel.valueType ===
                            SettingValueTypeEnum[SettingValueTypeEnum.Bool]
                        "
                        v-model:value="valueModel.boolValue"
                        :size="'large'"
                    >
                        <template #checked> true </template>
                        <template #unchecked> false </template>
                    </n-switch>
                </n-form-item>
                <n-form-item
                    v-if="newSettingModel.isFromExternalSource"
                    path="externalSourceType"
                    label="Тип внешнего источника"
                >
                    <n-select
                        v-model:value="newSettingModel.externalSourceType"
                        @keydown.enter.prevent
                        :options="externalSourceTypes"
                        :placeholder="'Выберите тип внешнего источника'"
                    />
                </n-form-item>
                <n-form-item
                    v-if="newSettingModel.isFromExternalSource"
                    path="externalSourcePath"
                    label="Путь до значения настройки во внешнем источнике"
                >
                    <n-input
                        v-model:value="newSettingModel.externalSourcePath"
                        @keydown.enter.prevent
                        :placeholder="'Укажите путь до настройки'"
                    />
                </n-form-item>
                <div v-if="newSettingModel.isFromExternalSource">
                    <a
                        v-if="newSettingModel.externalSourcePath"
                        :href="newSettingModel.externalSourcePath"
                        >{{ newSettingModel.externalSourcePath }}</a
                    >
                </div>
                <n-form-item
                    v-if="newSettingModel.isFromExternalSource"
                    path="externalSourceKey"
                    label="Ключ значения настройки во внешнем источнике (опционально)"
                >
                    <n-input
                        v-model:value="newSettingModel.externalSourceKey"
                        @keydown.enter.prevent
                        :placeholder="'Введите ключ настройки во внешнем источнике'"
                    />
                </n-form-item>
                <n-form-item
                    path="applications"
                    label="Приложения, для которых будет действовать настройка"
                >
                    <n-select
                        v-model:value="applicationSelected"
                        @keydown.enter.prevent
                        multiple
                        :options="applicationList"
                        :placeholder="'Выберите приложения'"
                    />
                </n-form-item>
            </n-form>
        </n-modal>

        <n-modal
            :show="showDeleteDialog"
            preset="confirm"
            title="Удаление приложения"
            positive-text="Удалить"
            negative-text="Отмена"
            @close="showDeleteDialog = false"
            @negative-click="showDeleteDialog = false"
            @positive-click="deleteSetting"
        >
            Вы действительно хотите удалить приложение
            {{ selectedSettingRowData?.name }}? Изменения необратимы
        </n-modal>
    </div>
</template>

<script setup lang="ts">
import router from "@/router";
import { ApplicationFrontModel } from "@/types/ApplicationFrontModel";
import {
    OperationResult,
    OperationResultGeneric,
} from "@/types/OperationResult";
import {
    ExternalSourceTypeEnum,
    SettingFrontModel,
    SettingItemFrontModel,
    SettingValueTypeEnum,
} from "@/types/SettingFrontModel";
import { AddEditDialogParameters } from "@/types/ShowAddEditDialogParameters";
import { createConfiguredFetch } from "@/utils/createConfiguredFetch";
import { notify } from "@/utils/notify";
import { Search12Regular } from "@vicons/fluent";
import {
    DataTableColumns,
    FormInst,
    FormRules,
    NButton,
    NCheckbox,
    NDataTable,
    NDivider,
    NForm,
    NFormItem,
    NH2,
    NIcon,
    NInput,
    NInputNumber,
    NModal,
    NSelect,
    NSpace,
    NSpin,
    NSwitch,
    NTag,
    useMessage,
    useNotification,
} from "naive-ui";
import { RowKey } from "naive-ui/es/data-table/src/interface";
import { OnUpdateValue } from "naive-ui/es/input/src/interface";
import { SelectBaseOption } from "naive-ui/es/select/src/interface";
import { computed, h, HTMLAttributes, ref, VNode, VNodeRef } from "vue";

interface CreateEditSettingModel extends Omit<SettingItemFrontModel, "key"> {
    key?: number | null;
}

const message = useMessage();
const notification = useNotification();

const settingFetch = createConfiguredFetch(
    message,
    process.env.VUE_APP_API_SETTINGS_URL
);

const appName = ref<string>(
    (router.currentRoute.value.query.appName as string) ?? ""
);

let isFetchingSettings = ref<boolean>(true);
const isFetchSuccess = ref<boolean>(false);
const allApplications = ref<ApplicationFrontModel[]>([]);

const urlParams = appName.value ? `?applicationName=${appName.value}` : "";

const getSettings = () => {
    const { isFetching, data, onFetchResponse, onFetchError } = settingFetch(
        `GetAll${urlParams}`
    )
        .get()
        .json<OperationResultGeneric<SettingFrontModel>>();

    isFetchingSettings = isFetching;

    onFetchResponse(() => {
        tableData.value = data.value?.entity?.settings ?? [];
        allApplications.value = data.value?.entity?.allApplications ?? [];
        isFetchSuccess.value = true;
    });

    onFetchError(() => {
        isFetchSuccess.value = false;
    });
};

getSettings();

const table = ref<VNodeRef | null>(null);
const tableData = ref<SettingItemFrontModel[]>([]);
const checkedRowKeys = ref<RowKey[]>([]);
const rowProps = (row: SettingItemFrontModel): HTMLAttributes => {
    return {
        style: "cursor: pointer;",
        onClick: () => {
            checkedRowKeys.value = [row.key];
        },
        onDblclick: () => {
            showEditDialog(row);
        },
    };
};

const settingValueTypes = ref(
    Object.entries(SettingValueTypeEnum)
        .filter((item) => isNaN(Number(item[0])))
        .map((item) => {
            return {
                label: item[0],
                value: item[0],
            };
        })
);
const externalSourceTypes = ref(
    Object.entries(ExternalSourceTypeEnum)
        .filter((item) => isNaN(Number(item[0])))
        .map((item) => {
            return {
                label: item[0],
                value: item[0],
            };
        })
);

const applicationSelected = ref<string[]>([]);

const formRef = ref<FormInst | null>(null);
const newSettingRules: FormRules = {
    name: [
        {
            required: true,
            validator(_, value: string) {
                if (!value) {
                    return new Error("Необходимо указать название");
                } else if (value.length < 2) {
                    return new Error(
                        "Название настройки должно состоять не меньше чем из 2-х символов"
                    );
                }
                return true;
            },
            trigger: ["input", "blur"],
        },
    ],
    description: [
        {
            required: true,
            validator(_, value: string) {
                if (!value) {
                    return new Error("Необходимо добавить описание");
                } else if (value.length < 2) {
                    return new Error(
                        "Описание настройки должно состоять не меньше чем из 2-х символов"
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

const applicationList = computed((): SelectBaseOption[] =>
    allApplications.value.map((app) => {
        return {
            label: app.name,
            value: app.name,
        };
    })
);

const newSettingModel = ref<CreateEditSettingModel>({
    name: "",
    description: "",
    value: "",
    valueType: SettingValueTypeEnum[SettingValueTypeEnum.String],
    applicationNames: [],
    isFromExternalSource: false,
    externalSourceType: null,
    externalSourcePath: "",
    externalSourceKey: "",
});

const valueModel = ref({
    boolValue: false,
    stringValue: "",
    numberValue: 0,
});

const clearValueModel = (): void => {
    valueModel.value.boolValue = false;
    valueModel.value.numberValue = 0;
    valueModel.value.stringValue = "";
};

const selectedSettingRowData = computed((): SettingItemFrontModel | null => {
    const key = checkedRowKeys.value?.[0];
    return tableData.value.find((app) => app.key == key) ?? null;
});

const mapValueToString = (valueType: string): string => {
    switch (valueType) {
        case SettingValueTypeEnum[SettingValueTypeEnum.String]:
            return valueModel.value.stringValue;
        case SettingValueTypeEnum[SettingValueTypeEnum.Bool]:
            return valueModel.value.boolValue ? "true" : "false";
        case SettingValueTypeEnum[SettingValueTypeEnum.Decimal]:
        case SettingValueTypeEnum[SettingValueTypeEnum.Double]:
        case SettingValueTypeEnum[SettingValueTypeEnum.Int]:
        case SettingValueTypeEnum[SettingValueTypeEnum.Long]:
            return valueModel.value.numberValue.toString();
        default:
            return "";
    }
};

const mapValueFromString = (valueType: string, value: string): void => {
    clearValueModel();

    switch (valueType) {
        case SettingValueTypeEnum[SettingValueTypeEnum.String]:
            valueModel.value.stringValue = value;
            break;
        case SettingValueTypeEnum[SettingValueTypeEnum.Bool]:
            valueModel.value.boolValue = value === "true";
            break;
        case SettingValueTypeEnum[SettingValueTypeEnum.Decimal]:
        case SettingValueTypeEnum[SettingValueTypeEnum.Double]:
        case SettingValueTypeEnum[SettingValueTypeEnum.Int]:
        case SettingValueTypeEnum[SettingValueTypeEnum.Long]:
            valueModel.value.numberValue = +value;
    }
};

const showAddDialog = () => {
    newSettingModel.value.name = "";
    newSettingModel.value.description = "";
    newSettingModel.value.value = "";
    newSettingModel.value.valueType =
        SettingValueTypeEnum[SettingValueTypeEnum.String];
    newSettingModel.value.applicationNames = [];
    newSettingModel.value.isFromExternalSource = false;
    newSettingModel.value.externalSourceType =
        ExternalSourceTypeEnum[ExternalSourceTypeEnum.Vault];
    newSettingModel.value.externalSourcePath = "";
    newSettingModel.value.externalSourceKey = "";

    valueModel.value.stringValue = "";
    valueModel.value.numberValue = 0;
    valueModel.value.boolValue = false;

    applicationSelected.value = [];

    addEditDialogParameters.value.title = "Добавление новой настройки";
    addEditDialogParameters.value.positiveText = "Добавить";
    addEditDialogParameters.value.positiveClick = () =>
        addSetting(newSettingModel.value);
    showAddEditDialog.value = true;
};

const showEditDialog = (rowToEdit: SettingItemFrontModel | null) => {
    newSettingModel.value.key = rowToEdit?.key;
    newSettingModel.value.name = rowToEdit?.name ?? "";
    newSettingModel.value.description = rowToEdit?.description ?? "";
    newSettingModel.value.value = rowToEdit?.value ?? "";
    newSettingModel.value.valueType =
        rowToEdit?.valueType ??
        SettingValueTypeEnum[SettingValueTypeEnum.String];
    newSettingModel.value.applicationNames = rowToEdit?.applicationNames ?? [];
    newSettingModel.value.isFromExternalSource =
        rowToEdit?.isFromExternalSource ?? false;
    newSettingModel.value.externalSourceType = rowToEdit?.externalSourceType;
    newSettingModel.value.externalSourcePath = rowToEdit?.externalSourcePath;
    newSettingModel.value.externalSourceKey = rowToEdit?.externalSourceKey;

    mapValueFromString(
        newSettingModel.value.valueType,
        newSettingModel.value.value
    );

    applicationSelected.value = applicationList.value
        .filter((app) =>
            newSettingModel.value.applicationNames?.includes(
                app.label as string
            )
        )
        .map((app) => app.label as string);

    addEditDialogParameters.value.title = "Редактирование настройки";
    addEditDialogParameters.value.positiveText = "Сохранить";
    addEditDialogParameters.value.positiveClick = () =>
        editSetting(newSettingModel.value);
    showAddEditDialog.value = true;
};

const addSetting = (setting: CreateEditSettingModel): void => {
    formRef.value
        ?.validate()
        .then(() => {
            showAddEditDialog.value = false;
            setting.value = mapValueToString(setting.valueType);

            const { data, onFetchResponse } = settingFetch("Add")
                .post(setting)
                .json<OperationResultGeneric<SettingItemFrontModel>>();

            onFetchResponse(() => {
                if (data.value?.entity) {
                    tableData.value.push(data.value.entity);
                }
                notify(notification, "success", "Добавлена новая настройка");
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

const editSetting = (setting: CreateEditSettingModel): void => {
    formRef.value
        ?.validate()
        .then(() => {
            showAddEditDialog.value = false;

            setting.value = mapValueToString(setting.valueType);

            setting.applicationNames = allApplications.value
                .map((app) => app.name)
                .filter((appName) =>
                    applicationSelected.value.includes(appName)
                );

            const { data, onFetchResponse } = settingFetch("Edit")
                .put(setting)
                .json<OperationResultGeneric<SettingItemFrontModel>>();

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

const deleteSetting = () => {
    showDeleteDialog.value = false;

    const { onFetchResponse } = settingFetch(
        `Delete/${selectedSettingRowData.value?.key}`
    )
        .delete()
        .json<OperationResult>();

    onFetchResponse(() => {
        tableData.value =
            tableData.value?.filter(
                (x) => x.key != selectedSettingRowData.value?.key
            ) ?? [];
        notify(notification, "success", "Настройка удалена");
    });
};

const columns: DataTableColumns<SettingItemFrontModel> = [
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
    {
        title: "Из внешнего источника",
        key: "isFromExternalSource",
        align: "center",
        render(row) {
            return h(
                NCheckbox,
                {
                    checked: row.isFromExternalSource,
                    disabled: true,
                },
                {
                    default: () => "",
                }
            );
        },
    },
    {
        title: "Значение",
        key: "value",
    },
    {
        title: "Ссылка на внешний источник",
        key: "externalSourcePath",
    },
    {
        title: "Ключ во внешнем источнике",
        key: "externalSourceKey",
    },
    {
        title: "Привязанные приложения",
        key: "applications",
        render(row) {
            return renderAppList(row.applicationNames);
        },
    },
];

const filterByName: OnUpdateValue = (value) => {
    table.value.filter({
        name: value,
    });
};

const renderAppList = (
    appNames: string[] | null | undefined
): VNode[] | null => {
    if (!appNames) return null;

    const tags = appNames.map((app) => {
        return h(
            NTag,
            {
                style: {
                    marginRight: "6px",
                },
                type: "info",
                bordered: false,
            },
            {
                default: () => app,
            }
        );
    });
    return tags;
};
</script>

<style scoped></style>
