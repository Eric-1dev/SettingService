<template>
    <n-config-provider :theme="isDarkTheme ? darkTheme : lightTheme">
        <n-layout :position="'absolute'" has-sider>
            <n-layout-header bordered>
                <n-space :justify="'space-between'" :align="'center'">
                    <div></div>
                    <n-h2>Сервис настроек</n-h2>
                    <n-space :justify="'end'" :align="'center'">
                        <n-button @click="toggleTheme">
                            {{ isDarkTheme ? "Светлая тема" : "Тёмная тема" }}
                        </n-button>
                    </n-space>
                </n-space>
            </n-layout-header>
            <n-layout has-sider position="absolute" style="top: 48px">
                <n-layout-sider
                    bordered
                    collapse-mode="width"
                    :collapsed="collapsed"
                    show-trigger
                    @collapse="collapsed = true"
                    @expand="collapsed = false"
                >
                    <n-menu :options="menuOptions"></n-menu>
                </n-layout-sider>
                <n-layout-content>
                    <slot></slot>
                </n-layout-content>
            </n-layout>
        </n-layout>
    </n-config-provider>
</template>

<script setup lang="ts">
import {
    NLayout,
    NButton,
    NConfigProvider,
    darkTheme,
    lightTheme,
    NLayoutHeader,
    NMenu,
    MenuOption,
    NSpace,
    NLayoutSider,
    NLayoutContent,
    NH2,
} from "naive-ui";
import { ref, h } from "vue";
import { RouterLink } from "vue-router";

const isDarkTheme = ref<boolean>(localStorage.isDarkTheme === "true");
const collapsed = ref(false);

const toggleTheme = (): void => {
    isDarkTheme.value = !isDarkTheme.value;
    localStorage.isDarkTheme = isDarkTheme.value;
};

const menuOptions = ref<MenuOption[]>([
    {
        label: () =>
            h(
                RouterLink,
                {
                    to: { name: "home-page" },
                },
                { default: () => "Домашняя страница" }
            ),
        key: "home",
    },
    {
        key: "home-divider",
        type: "divider",
        props: {
            style: {
                marginLeft: "32px",
            },
        },
    },
    {
        label: () =>
            h(
                RouterLink,
                {
                    to: { name: "settings-page" },
                },
                { default: () => "Настройки" }
            ),
        key: "settings",
    },
    {
        label: "Приложения",
        key: "applications",
        children: [
            {
                label: () =>
                    h(
                        RouterLink,
                        {
                            to: {
                                name: "applications-page",
                            },
                        },
                        { default: () => "Управление приложениями" }
                    ),
                key: "manage-applications",
            },
            {
                label: () =>
                    h(
                        RouterLink,
                        {
                            to: {
                                name: "applications-page",
                                query: {
                                    appName: "appName1",
                                },
                            },
                        },
                        { default: () => "AppName1" }
                    ),
                key: "app-name-1",
            },
            {
                label: "Sheep Man",
                key: "sheep-man",
            },
        ],
    },
    {
        label: () =>
            h(
                RouterLink,
                {
                    to: { name: "users-page" },
                },
                { default: () => "Пользователи" }
            ),
        key: "users",
    },
]);
</script>

<style>
* {
    padding: 0px;
    margin: 0px;
    border: none;
}

*,
*::before,
*::after {
    box-sizing: border-box;
}
</style>
