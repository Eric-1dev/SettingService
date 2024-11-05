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
                                @collapse="menuCollapseTrigger(true)"
                                @expand="menuCollapseTrigger(false)"
                            >
                                <n-menu
                                    :options="menuOptions"
                                    :icon-size="24"
                                    :collapsed-icon-size="32"
                                ></n-menu>
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
    Apps28Regular,
    Home32Regular,
    PeopleTeam32Regular,
    Settings32Regular,
} from "@vicons/fluent";
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
    NIcon,
} from "naive-ui";
import { ref, h, Component } from "vue";
import { RouterLink } from "vue-router";

interface Props {
    pageName: string;
}

const isDarkTheme = ref<boolean>(localStorage.isDarkTheme === "true");
const collapsed = ref(localStorage.menuCollapsed === "true");

const menuCollapseTrigger = (isCollapsed: boolean): void => {
    localStorage.menuCollapsed = isCollapsed;
    collapsed.value = isCollapsed;
};

const toggleTheme = (): void => {
    isDarkTheme.value = !isDarkTheme.value;
    localStorage.isDarkTheme = isDarkTheme.value;
};

const props = defineProps<Props>();

const renderIcon = (icon: Component) => {
    return () => h(NIcon, null, { default: () => h(icon) });
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
        icon: renderIcon(Home32Regular),
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
        icon: renderIcon(Settings32Regular),
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
        icon: renderIcon(Apps28Regular),
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
        icon: renderIcon(PeopleTeam32Regular),
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
