<template>
    <div>
        <div v-if="isFetching">Загрузка</div>
        <div v-else-if="data?.length === 0">Ни одно приложения пока не создано</div>
        <n-data-table v-else :columns="columns" :data="data ?? []" :row-props="rowProps"
            :bordered="false"></n-data-table>
    </div>
</template>

<script lang="ts" setup>

import { ApplicationForList } from "@/types/applicationForList";
import { useFetch } from "@vueuse/core";
import { DataTableColumns, NButton, NDataTable, NPopconfirm, useMessage } from "naive-ui";
import { CreateRowProps, RowData } from "naive-ui/es/data-table/src/interface";
import { h } from "vue";

const { isFetching, data, onFetchError } = useFetch("http://localhost:5186/api/Applications/GetAllApplications")
    .get()
    .json<ApplicationForList[]>();

onFetchError((error) => {
    console.log(error)
});

// window.$message = useMessage()

const rowProps: CreateRowProps = (row: RowData) => {
    return {
        style: 'cursor: pointer; user-select: none',
        onClick: () => {
            // window.$message.info(row.name)
        }
    }
}

const columns: DataTableColumns<ApplicationForList> =
    [
        {
            title: 'ID',
            key: 'id'
        },
        {
            title: 'Название',
            key: 'name'
        },
        {
            title: 'Описание',
            key: 'description'
        },
        {
            key: 'actions',
            render(row) {
                return h(
                    NPopconfirm,
                    {
                        strong: true,
                        size: 'small',
                        positiveText: 'Да',
                        negativeText: 'Нет',
                        onPositiveClick: () => alert(row.id)
                    },
                    {
                        trigger: () => h(NButton, { ghost: true, type: "error" }, { default: () => "Удалить" }),
                        default: () => `Вы действительно хотите удалить приложение ${row.name}? Изменения необратимы`
                    }
                )
            }
        }
    ]

</script>

<style scoped></style>
