<template>
  <v-header>
    <v-app-bar app color="#9FC9FC" scroll-behavior="hide" elevation="0">
      <v-app-bar-title>
        <v-btn variant="text">Bienvenido a la Casa Del Café de Barbo</v-btn>
      </v-app-bar-title>
    </v-app-bar>
  </v-header>

  <v-container>
    <v-row>
      <!-- Columna de Cafés -->
      <v-col cols="12" md="6">
        <v-subheader>Catálogo de Cafés</v-subheader>
        <template v-if="coffees.length > 0">
          <v-row>
            <v-col v-for="coffee in coffees" :key="coffee.type" cols="12">
              <v-card>
                <v-card-title class="d-flex justify-space-between">
                  <span>{{ coffee.type }}</span>
                </v-card-title>
                <v-card-subtitle>
                  ₡{{ coffee.price }} - Stock: {{ coffee.stock }}
                </v-card-subtitle>
                <v-card-actions>
                  <v-btn color="primary" @click="addToCart(coffee)">Agregar a la orden</v-btn>
                </v-card-actions>
              </v-card>
            </v-col>
          </v-row>
        </template>

        <v-alert v-else type="info">No hay cafés disponibles.</v-alert>
      </v-col>

      <!-- Columna del Carrito -->
      <v-col cols="12" md="6">
        <v-subheader>Su Orden</v-subheader>
        <v-list v-if="cart.length > 0">
          <v-list-item-group>
            <v-list-item v-for="(item, index) in cart" :key="index">
              <v-list-item-content>
                <v-list-item-title>{{ item.type }} x{{ item.quantity }} - ₡{{ item.total }}</v-list-item-title>
              </v-list-item-content>
            </v-list-item>
          </v-list-item-group>
        </v-list>

        <v-alert v-else type="info">Su carrito está vacío.</v-alert>

        <v-divider v-if="cart.length > 0"></v-divider>

        <v-row v-if="cart.length > 0">
          <v-col>
            <v-btn color="primary" @click="makePurchase">Comprar</v-btn>
          </v-col>
          <v-col class="text-right">
            <v-subheader>Total: ₡{{ totalCost }}</v-subheader>
          </v-col>
        </v-row>
        
        <!-- Input para las monedas y billetes -->
        <v-row v-if="cart.length > 0">
          <v-col cols="12">
            <v-subheader>Ingrese cantidad de monedas/billetes</v-subheader>
            <v-text-field
              label="Monedas de 1000"
              v-model.number="manualCoinsInput[1000]"
              type="number"
              :min="0"
              required
            ></v-text-field>
            <v-text-field
              label="Monedas de 500"
              v-model.number="manualCoinsInput[500]"
              type="number"
              :min="0"
              required
            ></v-text-field>
            <v-text-field
              label="Monedas de 100"
              v-model.number="manualCoinsInput[100]"
              type="number"
              :min="0"
              required
            ></v-text-field>
            <v-text-field
              label="Monedas de 50"
              v-model.number="manualCoinsInput[50]"
              type="number"
              :min="0"
              required
            ></v-text-field>
            <v-text-field
              label="Monedas de 25"
              v-model.number="manualCoinsInput[25]"
              type="number"
              :min="0"
              required
            ></v-text-field>
          </v-col>
        </v-row>

        <!-- Mensajes de error y desglose del vuelto -->
        <v-row v-if="errorMessage">
          <v-col>
            <v-alert type="error" :value="errorMessage">{{ errorMessage }}</v-alert>
          </v-col>
        </v-row>
        <v-row v-if="changeBreakdown">
          <v-col>
            <h4>Desglose del Vuelto:</h4>
            <v-list dense>
              <v-list-item v-for="(count, denomination) in changeBreakdown" :key="denomination">
                <v-list-item-content>
                  <v-list-item-title>{{ denomination }} colones: {{ count }}</v-list-item-title>
                </v-list-item-content>
              </v-list-item>
            </v-list>
          </v-col>
        </v-row>

        <v-row v-if="changeAmount !== null">
          <v-col>
            <v-alert type="success">Vuelto: ₡{{ changeAmount }}</v-alert>
          </v-col>
        </v-row>
      </v-col>
    </v-row>
  </v-container>
</template>

<script>
import api from '@/axios';

export default {
  data() {
    return {
      coffees: [],
      cart: [],
      paidAmount: 0,
      totalCost: 0,
      changeAmount: null,
      changeBreakdown: {},
      errorMessage: '',
      manualCoinsInput: {
        1000: 0,
        500: 0,
        100: 0,
        50: 0,
        25: 0
      }
    };
  },
  created() {
    this.fetchCoffees();
  },
  methods: {
    async fetchCoffees() {
      try {
        const response = await api.get('https://localhost:7227/api/Coffees/GetCoffees');
        this.coffees = response.data;
      } catch (error) {
        console.error('Error fetching coffees:', error);
      }
    },
    addToCart(coffee) {
      const found = this.cart.find(item => item.type === coffee.type);
      if (found) {
        found.quantity++;
        found.total = found.quantity * found.price;
      } else {
        this.cart.push({
          type: coffee.type,
          price: coffee.price,
          quantity: 1,
          total: coffee.price,
        });
      }
      this.calculateTotalCost();
    },
    calculateTotalCost() {
      let total = 0;
      this.cart.forEach(item => {
        total += item.total;
      });
      this.totalCost = total;
    },
    calculateTotalPaidAmount() {
      let total = 0;
      // Recorremos cada clave y valor en el objeto
      Object.entries(this.manualCoinsInput).forEach(([denomination, quantity]) => {
        // Para cada denominación, sumamos la cantidad de monedas/billetes multiplicado por la denominación
        total += denomination * quantity;
      });

      this.paidAmount = total;
    },
    async makePurchase() {
      // Verificar si el pago es suficiente
      console.log('Monto a pagar: ₡', this.totalCost);
      console.log('Monto pagado: ₡', this.paidAmount);
      this.calculateTotalPaidAmount();
      if (this.paidAmount < this.totalCost) {
        this.errorMessage = 'El monto pagado es insuficiente';
        return;
      }

      // Si el pago es suficiente, se realiza la compra
      const requestData = {
        purchaseQuantities: this.cart.reduce((acc, item) => {
          acc[item.type] = item.quantity;
          return acc;
        }, {}),
        paidAmount: this.paidAmount,
        manualCoinsInput: this.manualCoinsInput
      };

      try {
        const response = await api.post('https://localhost:7227/api/Coffees/Purchase', requestData);
        const data = response.data;

        // Mostrar mensaje y desglose del vuelto
        this.$swal.fire({
          title: 'Compra Exitosa',
          html: `
            <p>¡Gracias por su compra!</p>
            <p>Total Pagado: ₡${this.paidAmount}</p>
            <p>Vuelto: ₡${data.change.total}</p>
            <h4>Desglose del Vuelto:</h4>
            <ul>
              ${Object.entries(data.change.breakdown)
                .map(([denomination, count]) => `<li>${denomination} colones: ${count}</li>`)
                .join('')}
            </ul>
          `,
          icon: 'success',
          confirmButtonText: 'Entendido'
        });

        // Actualizar valores locales
        this.cart = [];
        this.paidAmount = 0;
        this.changeAmount = data.change.total;
        this.changeBreakdown = data.change.breakdown;
        this.coffees = data.remainingCoffees;
      } catch (error) {
        const errorMessage = error.response?.data?.Error || 'Error de Stock, no hay suficientes';
        this.$swal.fire({
          title: 'Error en la Compra',
          text: errorMessage,
          icon: 'error',
          confirmButtonText: 'Intentar de Nuevo'
        });

        this.errorMessage = errorMessage;
      }
    }
  }
};
</script>

<style scoped>
.error {
  color: red;
  font-weight: bold;
}
</style>
