<template>
    <n-config-provider :theme="isDarkTheme ? darkTheme : lightTheme">
        <n-theme-editor>
            <n-message-provider :placement="'bottom-right'">
                <n-notification-provider>
                    <n-layout
                        :position="'absolute'"
                        content-style="padding: 0 5px 0 5px;"
                    >
                        <n-layout-header bordered>
                            <n-space
                                :justify="'space-between'"
                                :align="'baseline'"
                            >
                                <n-h2>Сервис настроек</n-h2>
                                <n-space :justify="'end'">
                                    <n-button @click="toggleTheme">
                                        {{
                                            isDarkTheme
                                                ? "Светлая тема"
                                                : "Тёмная тема"
                                        }}
                                    </n-button>
                                </n-space>
                            </n-space>
                        </n-layout-header>
                        <n-layout
                            has-sider
                            position="absolute"
                            style="top: 42px"
                        >
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
                            <n-layout-content style="padding: 0 5px 0 5px">
                                <n-space :justify="'center'" :align="'center'"
                                    ><n-h2>{{ props.pageName }}</n-h2></n-space
                                >
                                <slot></slot>
                            </n-layout-content>
                        </n-layout>
                    </n-layout>
                </n-notification-provider>
            </n-message-provider>
        </n-theme-editor>
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
    NMessageProvider,
    NNotificationProvider,
    NThemeEditor,
} from "naive-ui";
import { ref, h, computed } from "vue";
import { RouterLink } from "vue-router";

interface Props {
    pageName: string;
}

const isDarkTheme = ref<boolean>(localStorage.isDarkTheme === "true");
const collapsed = ref(false);

const toggleTheme = (): void => {
    isDarkTheme.value = !isDarkTheme.value;
    localStorage.isDarkTheme = isDarkTheme.value;
};

const props = defineProps<Props>();

const menuOptions = computed<MenuOption[]>(() => [
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
        label: () =>
            h(
                RouterLink,
                {
                    to: { name: "applications-manage-page" },
                },
                { default: () => "Приложения" }
            ),
        key: "applications",
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

.ss-main-container {
    position: fixed;
    top: 0;
    bottom: 0;
    right: 0;
    left: 0;
    display: flex;
    flex-direction: column;
}

.ss-header-container {
    height: 100px;
}

.ss-work_field-container {
    display: flex;
    flex-direction: row;
}

.ss-sidebar-container {
    width: 300px;
}

.ss-content-container {
    height: 100%;
    width: 100%;
}
</style>
