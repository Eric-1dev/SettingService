<template>
    <div>
        <div>Список приложений</div>
        <div v-if="isLoading">Загрузка</div>
        <div v-else-if="apps?.length === 0">Ни одно приложения пока не создано</div>
        <n-data-table v-else :columns="columns" :data="apps" :bordered="false"></n-data-table>
    </div>
</template>

<script lang="ts" setup>
import { ApplicationForList } from "@/types/applicationForList";
import { useFetch } from "@vueuse/core";
import { DataTableColumns, NDataTable } from "naive-ui";
import { RowData } from "naive-ui/es/data-table/src/interface";
import { ref } from "vue";

const { isFetching, error, data } = useFetch<ApplicationForList[]>("http://localhost:5186/api/Applications/GetAllApplications")
    .get()
    .json<ApplicationForList[]>();

const apps = ref(data);
const columns: DataTableColumns<ApplicationForList> =
    [
        {
            title: 'Название',
            key: 'name'
        },
        {
            title: 'Описание',
            key: 'description'
        },
    ]

const isLoading = ref(isFetching);
</script>

<style scoped></style>
