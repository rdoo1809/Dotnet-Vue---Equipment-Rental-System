<template>
  <div id="app">
    <Layout v-if="isAuthenticated" />
    <router-view v-else />
    <Toast ref="toast" />
  </div>
</template>

<script setup>
import { computed, provide, ref } from 'vue'
import { useAuthStore } from '@/stores/auth'
import Layout from '@/components/Layout.vue'
import Toast from '@/components/Toast.vue'

const authStore = useAuthStore()
const isAuthenticated = computed(() => authStore.isAuthenticated)

// Provide toast functionality to all components
const toast = ref(null)
provide('toast', toast)
</script>
