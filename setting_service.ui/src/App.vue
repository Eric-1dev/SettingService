<template>
    <ss-layout :key="$route.fullPath" :pageName="pageName ?? ''">
        <router-view> </router-view>
    </ss-layout>
</template>

<script setup lang="ts">
import { computed } from "vue";
import SsLayout from "./components/SsLayout.vue";
import { useRoute } from "vue-router";

const route = useRoute();

const pageName = computed(() => {
    const name = (route.meta.pageName as string) ?? "";
    document.title = name;
    return name;
});
</script>

<style></style>
