<template>
  <ss-layout :pageName="pageName ?? ''">
    <router-view></router-view>
  </ss-layout>
  <n-message-provider></n-message-provider>
</template>

<script setup lang="ts">

import { computed } from 'vue';
import SsLayout from './components/SsLayout.vue';
import { useRoute } from 'vue-router';
import { NMessageProvider } from 'naive-ui';

const route = useRoute()

const pageName = computed(() => {
  const name = route.meta.pageName as string
  document.title = name
  return name
})

</script>

<style></style>