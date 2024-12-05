<template>
  <v-app>
    <v-main>
      <VendingMachine/>
    </v-main>
  </v-app>
</template>

<script>
import VendingMachine from './components/VendingMachine.vue'

export default {
  name: 'App',

  components: {
    VendingMachine,
  },

  data: () => ({
    //
  }),
}
</script>
